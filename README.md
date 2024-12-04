# Tibber Robot 🤖

The Tibber Robot service simulates a robot moving in an office space
and cleans the places this robot visits. The path of the robot's movement is
described by the starting coordinates and move commands. After the cleaning has
been done, the robot reports the number of unique places cleaned. The service stores
the results into the database and returns the created record in JSON format. The
service listens to the HTTP protocol on port 5000.

## Getting started 🚀

### Prerequisites 🛠️

The following tools are required to run the service:
- Microsoft .NET 8.0 SDK
- Docker

### .NET Aspire 👩‍💻

The easiest way to run the service is by launching the .NET Aspire App Host.

1. Run the following command from the root of the repository:
`dotnet run --project ./Tibber.Robot.AppHost/`
2. Click on the link in the console to login to the dashboard

The dashboard provides an overview of the running services and their status. You can view their health, logs, traces and metrics.

- Click on the endpoint of the service named `api` to get to the API documentation page. From here you can test the service by sending requests to the API.
- Click on the endpoint of the service named `dbserver-pgadmin` to get to the pgAdmin dashboard. From here you can easily access the database and view the stored records.

### Docker compose 🐳

The service can be run using Docker compose. The following command will build the service and run it in a container:

- `docker build -f "Tibber.Robot.Api/Dockerfile" -t tibberrobotapi .`
- `docker compose up`

## Tests 🧪

The service has unit tests and integration tests or can be tested manually using the API documentation page or any HTTP client.

### Unit tests

The unit tests are to test the logic of the robot. The tests are located in the `Tibber.Robot.UnitTests` project. To run the tests, execute the following command from the root of the repository: `dotnet test ./Tibber.Robot.UnitTests/`

### Integration tests

The integration tests are to test the API endpoint. It ensures the API returns the execution result from the API and that the result is stored in the database.

The tests are located in the `Tibber.Robot.IntegrationTests` project. To run the tests, execute the following command from the root of the repository: `dotnet test ./Tibber.Robot.IntegrationTests/`

### Manual testing

The API documentation page can be used to test the service manually or any HTTP client can be used to send requests to the service.

There's a sample HTTP request to test the service in at the following location: `Tibber.Robot.Api/Tibber.Robot.Api.http`. The file can be opened in Visual Studio or Visual Studio Code with the REST Client extension installed.

## Performance optimization 📊

The initial implementation of the robot performed adequately for a small number of commands. However, as the number of commands increased, performance degraded significantly. This performance was unacceptable for the use case represented by the [robotcleanerpathheavy.json](Tibber.Robot.Benchmarks/robotcleanerpathheavy.json).

Improving performance proved challenging. Several strategies were explored before settling on the most efficient solution. Below is an outline of these strategies, followed by an overview of their performance. Each strategy is included in the repository and can be executed using the benchmarks project:

```bash
dotnet run --project ./Tibber.Robot.Benchmarks/ --configuration release --filter '*'
```

### HashRobot

This approach used the initial strategy to calculate unique positions with a hash set. While hash sets are effective for storing unique values, they became inefficient as the number of unique positions increased. Profiling revealed that the `HashSet.Add` method accounted for 90% of CPU usage. Attempts to optimize the hash function and equality comparer yielded only marginal improvements.

### VectorRobot

To improve performance, this strategy avoided storing each unique position and instead stored movement vectors. Specifically, it focused on horizontal vectors, identifying overlaps and merging them. The result was a list of non-overlapping horizontal vectors, which allowed for quick calculation of unique positions by summing their lengths. While this strategy outperformed the hash-based approach for larger number of commands, it was still too slow for the heavy use case.

### SegmentedRobot

This strategy divided the office space into 100x100 segments. The robot determined its current segment with a simple calculation and used a hash set within each segment to track unique positions. A boolean flag indicated whether an entire segment had been cleaned, allowing the hash set’s memory to be released. By reducing the number of unique values stored in each hash set and simplifying the data structure, this strategy achieved better performance than the hash-based approach. However, it was slower than the vector-based strategy.

### BentleyOttmannRobot

Building on the vector approach, this strategy aimed to handle both horizontal and vertical movements more efficiently. It stored all movement paths and used the Bentley-Ottmann algorithm to calculate intersections. This algorithm is well-known for its efficiency in managing overlapping paths and proved to be the fastest of all strategies, particularly for the heavy use case.

An additional optimization attempt involved pre-merging paths before passing them to the Bentley-Ottmann algorithm, reducing its workload. However, this added complexity yielded only slight performance gains for extremely large command sets.

### Conclusion

The following table summarizes the benchmark results for each strategy. Benchmarks were conducted on a Surface Laptop Studio running Windows 11 with an 11th Gen Intel Core i7-11370H (3.30 GHz), 1 CPU, 8 logical cores, and 4 physical cores.

- Light case: Simple scenario with 4 commands.
- Medium case: More complex scenario with 250 commands.
- Heavy case: Highly complex scenario with 10,000 commands.

The heavy case was only benchmarked for the Bentley-Ottmann robots, as the other variants clearly underperformed in the light and medium cases.

#### Performance Comparison (Mean Execution Time):
1. **Bentley-Ottmann** variants consistently outperform all other methods in terms of execution time, especially in the light and medium cases.
    - **Light case**: The Bentley-Ottmann method is over 6,800x faster than the slowest method (SegmentedRobot).
    - **Medium case**: It is significantly faster (over 100x) than the next fastest non-Bentley-Ottmann method (VectorRobot).
    - **Heavy case**: Both Bentley-Ottmann variants perform similarly, with only a slight improvement for the optimized version.

2. **Optimized Bentley-Ottmann** shows minor improvements in the heavy case (faster by ~4%), but it is slightly slower in the light and medium cases due to added overhead for pre-merging paths.

3. The **HashRobot**, **SegmentedRobot**, and **VectorRobot** are inefficient, with execution times orders of magnitude slower than the Bentley-Ottmann variants.
    - Light case: These methods take 47,000–62,000 μs, compared to just 6–10 μs for the Bentley-Ottmann variants.
    - Medium and heavy cases: Their performance becomes increasingly unmanageable as the command count increases.

#### Memory Usage (Allocated):
1. **Bentley-Ottmann Variants**
    - For the **light case**, memory usage is minimal (**8.52–15 KB**) and scales reasonably for the **medium case** (**~3.5–4 MB**).
    - For the **heavy case**, memory usage jumps to **~5.5 GB**, indicating exponential growth relative to the command count.

2. **Other Methods**
    - Memory usage for the **light case** is already high (**22–54 MB**), which is disproportionate for just 4 commands.
    - For the **medium case**, memory usage becomes excessive (**233 MB to 2.45 GB**), reflecting poor scalability.

#### Garbage Collection (GC) Events:
1. **Bentley-Ottmann Variants**
    - GC events remain relatively low for the light and medium cases, but for the heavy case, 767,000+ events in Gen0 indicate significant memory allocation pressures due to the large dataset.

2. **Other Methods**
    - GC events are consistently high, even for small workloads, highlighting inefficiencies in memory management.

#### Command Count vs. Performance Trends:
1. **Bentley-Ottmann Variants:**
    - **Execution time grows non-linearly** but remains feasible even as the command count increases. For example:
        - **Light case** (**4 commands**): **~7 μs**
        - **Medium case** (**250 commands**): **~6,348 μs** (**~900x increase** for **62.5x more commands**)
        - **Heavy case** (**10,000 commands**): **~14.97 s** (**~2,300x increase** for **40x more commands**)
    
    This demonstrates that performance degradation is exponential for large workloads.

2. **Other Methods:**
    - Performance degrades dramatically even for the medium case, with execution times and memory usage increasing exponentially relative to the command count.
    - These methods are unsuitable for scaling beyond very small workloads (light case).

#### Key Insights:
1. **Bentley-Ottmann Variants Are Superior:**
    - These methods dominate in both execution time and memory efficiency across all tested cases. However, they exhibit **exponential performance degradation** with higher command counts, suggesting room for optimization in large-scale scenarios.

2. **Other Methods Are Unsustainable:**
    -HashRobot, SegmentedRobot, and VectorRobot are impractical for medium and heavy cases due to their **unacceptable performance and memory usage**.

#### Focus for Future Optimization:
    - Efforts should aim to reduce the **exponential growth in execution time and memory usage** for Bentley-Ottmann methods, particularly for heavy workloads. This might involve:
        - Further optimizing the algorithm for large datasets.
        - Exploring memory-efficient data structures.
        - Parallelizing or chunking the workload for better scalability.

| Method                        | Mean              | Error          | StdDev         | Gen0        | Gen1        | Gen2      | Allocated     |
|------------------------------ |------------------:|---------------:|---------------:|------------:|------------:|----------:|--------------:|
| LightBentleyOttmann           |          6.862 us |      0.1090 us |      0.0967 us |      1.3885 |      0.0153 |         - |       8.52 KB |
| LightOptimizedBentleyOttmann  |         10.387 us |      0.0926 us |      0.0866 us |      2.4414 |      0.0305 |         - |         15 KB |
| LightHashRobot                |     58,180.082 us |  1,162.0597 us |  2,501.4563 us |   2666.6667 |   1888.8889 | 1111.1111 |   34751.38 KB |
| LightSegmentedRobot           |     62,660.858 us |  1,229.7340 us |  1,802.5283 us |   5000.0000 |   3142.8571 | 1142.8571 |   54849.34 KB |
| LightVectorRobot              |     47,083.224 us |  1,139.4388 us |  3,305.7188 us |   3416.6667 |   2416.6667 | 1166.6667 |   22318.16 KB |
| MediumBentleyOttmann          |      6,348.525 us |     59.1235 us |     55.3041 us |    539.0625 |    273.4375 |  132.8125 |    3523.58 KB |
| MediumOptimizedBentleyOttmann |      7,317.029 us |    145.7295 us |    209.0010 us |    640.6250 |    343.7500 |  140.6250 |    4082.97 KB |
| MediumHashRobot               |  4,400,954.087 us | 34,273.8877 us | 32,059.8158 us | 104000.0000 |  56000.0000 | 9000.0000 | 2450389.57 KB |
| MediumSegmentedRobot          |  1,076,473.333 us | 11,450.0644 us | 10,710.3974 us | 111000.0000 |  70000.0000 | 9000.0000 | 1052212.68 KB |
| MediumVectorRobot             |    706,829.836 us | 13,217.4407 us | 11,716.9169 us |  42000.0000 |  24000.0000 | 6000.0000 |  233311.55 KB |
| HeavyBentleyOttmann           | 14,974,140.807 us | 61,889.9777 us | 54,863.8532 us | 767000.0000 | 266000.0000 | 9000.0000 | 5492758.89 KB |
| HeavyOptimizedBentleyOttmann  | 14,351,073.233 us | 32,508.1204 us | 30,408.1160 us | 768000.0000 | 266000.0000 | 8000.0000 | 5505240.19 KB |

## Considerations 🤔

Here are some considerations that were made during the development of the service:
- The API project contains the API endpoints as well as the database models and the database context. This is to keep the project simple and easy to understand.
- The API response uses the same model as the database model. Since both models would otherwise be the same, it's easier to use the same model for both. In a real-world scenario, the models would probably be (slightly) different and would it make sense to map between the two models.
- The database initialization is part of the startup sequence of the API. This is again to keep things simple and works fine because we have just one instance of the API. In a real-world scenario, where multiple instances of the API may start, the database should be initialized separately from the API start-up.
- As per the notes in the case documentation, all input is considered to be well formed, syntactically correct and the robot will never be sent outside the bounds of the office space. The API does not validate the input and assumes it's correct.
- There's a small typo in the example request in the case documentation. The example request uses `commmands` instead of `commands`. The API uses the correct spelling.