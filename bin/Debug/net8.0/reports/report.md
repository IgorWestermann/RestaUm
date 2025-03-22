# Algorithm Performance Analysis Report
Generated on: 20/03/2025 17:22:33

## Test Configuration
- Algorithm Iterations: 1
- Max Iterations Limit: 1000000
- Timeout: 180000ms
- Default Move Cost: 1
- Progress Update Interval: 1000

## Executive Summary
This report compares the performance of various search algorithms for the Peg Solitaire puzzle with and without hash verification. Hash verification helps detect symmetric board positions to avoid redundant exploration.

### Performance Summary
| Algorithm | Hash Verification | Success Rate | Avg Time (ms) | Avg Iterations | Avg Solution Depth |
|-----------|-------------------|--------------|---------------|----------------|-------------------|
| A* | Enabled | 1/1 (100%) | 631 | 2509 | 31,0 |
| A* | Disabled | 1/1 (100%) | 358 | 2206 | 31,0 |
| Best First Search | Enabled | 1/1 (100%) | 333 | 2509 | 31,0 |
| Best First Search | Disabled | 1/1 (100%) | 286 | 2206 | 31,0 |
| A* Weighted | Enabled | 1/1 (100%) | 23265 | 521266 | 31,0 |
| A* Weighted | Disabled | 1/1 (100%) | 21307 | 521266 | 31,0 |
| Ordered Search | Enabled | 1/1 (100%) | 44423 | 886308 | 31,0 |
| Ordered Search | Disabled | 1/1 (100%) | 45774 | 886308 | 31,0 |
| Depth First Search | Enabled | 1/1 (100%) | 1644 | 30785 | 31,0 |
| Depth First Search | Disabled | 1/1 (100%) | 1509 | 30785 | 31,0 |
| Backtracking | Enabled | 1/1 (100%) | 274 | 2313 | 31,0 |
| Backtracking | Disabled | 1/1 (100%) | 334 | 2313 | 31,0 |
| Breadth First Search | Enabled | 0/1 (0%) | 131327 | 1000000 | N/A |
| Breadth First Search | Disabled | 0/1 (0%) | 119188 | 1000000 | N/A |

### Hash Verification Impact
| Algorithm | Time Difference | Iteration Difference | Memory/State Savings |
|-----------|-----------------|----------------------|---------------------|
| A* | 273ms slower with hash (43% degradation) | 303 more iterations with hash (12% increase) | No memory savings |
| Best First Search | 47ms slower with hash (14% degradation) | 303 more iterations with hash (12% increase) | No memory savings |
| A* Weighted | 1958ms slower with hash (8% degradation) | -0 more iterations with hash (-0% increase) | No memory savings |
| Ordered Search | 1351ms faster with hash (3% improvement) | -0 more iterations with hash (-0% increase) | No memory savings |
| Depth First Search | 135ms slower with hash (8% degradation) | -0 more iterations with hash (-0% increase) | No memory savings |
| Backtracking | 60ms faster with hash (18% improvement) | -0 more iterations with hash (-0% increase) | No memory savings |
| Breadth First Search | 12139ms slower with hash (9% degradation) | -0 more iterations with hash (-0% increase) | No memory savings |

## Detailed Algorithm Results

### A*
#### With Hash Verification
- **Success Rate**: 1/1 (100%)
- **Execution Time**: Avg = 631ms, Min = 631ms, Max = 631ms
- **Iterations**: Avg = 2509, Min = 2509, Max = 2509
- **Solution Depth**: Avg = 31,0, Min = 31, Max = 31

**Individual Runs:**
| Run | Time (ms) | Iterations | Found Solution | Solution Depth |
|-----|-----------|------------|----------------|----------------|
| 1 | 631 | 2509 | Yes | 31 |

#### Without Hash Verification
- **Success Rate**: 1/1 (100%)
- **Execution Time**: Avg = 358ms, Min = 358ms, Max = 358ms
- **Iterations**: Avg = 2206, Min = 2206, Max = 2206
- **Solution Depth**: Avg = 31,0, Min = 31, Max = 31

**Individual Runs:**
| Run | Time (ms) | Iterations | Found Solution | Solution Depth |
|-----|-----------|------------|----------------|----------------|
| 1 | 358 | 2206 | Yes | 31 |

#### Analysis
- Hash verification made the algorithm **273ms slower** (43% degradation).
- With hash verification, the algorithm explored **303 more states** (12% increase).
- Success rate remained the same with hash verification.

### Best First Search
#### With Hash Verification
- **Success Rate**: 1/1 (100%)
- **Execution Time**: Avg = 333ms, Min = 333ms, Max = 333ms
- **Iterations**: Avg = 2509, Min = 2509, Max = 2509
- **Solution Depth**: Avg = 31,0, Min = 31, Max = 31

**Individual Runs:**
| Run | Time (ms) | Iterations | Found Solution | Solution Depth |
|-----|-----------|------------|----------------|----------------|
| 1 | 333 | 2509 | Yes | 31 |

#### Without Hash Verification
- **Success Rate**: 1/1 (100%)
- **Execution Time**: Avg = 286ms, Min = 286ms, Max = 286ms
- **Iterations**: Avg = 2206, Min = 2206, Max = 2206
- **Solution Depth**: Avg = 31,0, Min = 31, Max = 31

**Individual Runs:**
| Run | Time (ms) | Iterations | Found Solution | Solution Depth |
|-----|-----------|------------|----------------|----------------|
| 1 | 286 | 2206 | Yes | 31 |

#### Analysis
- Hash verification made the algorithm **47ms slower** (14% degradation).
- With hash verification, the algorithm explored **303 more states** (12% increase).
- Success rate remained the same with hash verification.

### A* Weighted
#### With Hash Verification
- **Success Rate**: 1/1 (100%)
- **Execution Time**: Avg = 23265ms, Min = 23265ms, Max = 23265ms
- **Iterations**: Avg = 521266, Min = 521266, Max = 521266
- **Solution Depth**: Avg = 31,0, Min = 31, Max = 31

**Individual Runs:**
| Run | Time (ms) | Iterations | Found Solution | Solution Depth |
|-----|-----------|------------|----------------|----------------|
| 1 | 23265 | 521266 | Yes | 31 |

#### Without Hash Verification
- **Success Rate**: 1/1 (100%)
- **Execution Time**: Avg = 21307ms, Min = 21307ms, Max = 21307ms
- **Iterations**: Avg = 521266, Min = 521266, Max = 521266
- **Solution Depth**: Avg = 31,0, Min = 31, Max = 31

**Individual Runs:**
| Run | Time (ms) | Iterations | Found Solution | Solution Depth |
|-----|-----------|------------|----------------|----------------|
| 1 | 21307 | 521266 | Yes | 31 |

#### Analysis
- Hash verification made the algorithm **1958ms slower** (8% degradation).
- With hash verification, the algorithm explored **-0 more states** (-0% increase).
- Success rate remained the same with hash verification.

### Ordered Search
#### With Hash Verification
- **Success Rate**: 1/1 (100%)
- **Execution Time**: Avg = 44423ms, Min = 44423ms, Max = 44423ms
- **Iterations**: Avg = 886308, Min = 886308, Max = 886308
- **Solution Depth**: Avg = 31,0, Min = 31, Max = 31

**Individual Runs:**
| Run | Time (ms) | Iterations | Found Solution | Solution Depth |
|-----|-----------|------------|----------------|----------------|
| 1 | 44423 | 886308 | Yes | 31 |

#### Without Hash Verification
- **Success Rate**: 1/1 (100%)
- **Execution Time**: Avg = 45774ms, Min = 45774ms, Max = 45774ms
- **Iterations**: Avg = 886308, Min = 886308, Max = 886308
- **Solution Depth**: Avg = 31,0, Min = 31, Max = 31

**Individual Runs:**
| Run | Time (ms) | Iterations | Found Solution | Solution Depth |
|-----|-----------|------------|----------------|----------------|
| 1 | 45774 | 886308 | Yes | 31 |

#### Analysis
- Hash verification made the algorithm **1351ms faster** (3% improvement).
- With hash verification, the algorithm explored **-0 more states** (-0% increase).
- Success rate remained the same with hash verification.

### Depth First Search
#### With Hash Verification
- **Success Rate**: 1/1 (100%)
- **Execution Time**: Avg = 1644ms, Min = 1644ms, Max = 1644ms
- **Iterations**: Avg = 30785, Min = 30785, Max = 30785
- **Solution Depth**: Avg = 31,0, Min = 31, Max = 31

**Individual Runs:**
| Run | Time (ms) | Iterations | Found Solution | Solution Depth |
|-----|-----------|------------|----------------|----------------|
| 1 | 1644 | 30785 | Yes | 31 |

#### Without Hash Verification
- **Success Rate**: 1/1 (100%)
- **Execution Time**: Avg = 1509ms, Min = 1509ms, Max = 1509ms
- **Iterations**: Avg = 30785, Min = 30785, Max = 30785
- **Solution Depth**: Avg = 31,0, Min = 31, Max = 31

**Individual Runs:**
| Run | Time (ms) | Iterations | Found Solution | Solution Depth |
|-----|-----------|------------|----------------|----------------|
| 1 | 1509 | 30785 | Yes | 31 |

#### Analysis
- Hash verification made the algorithm **135ms slower** (8% degradation).
- With hash verification, the algorithm explored **-0 more states** (-0% increase).
- Success rate remained the same with hash verification.

### Backtracking
#### With Hash Verification
- **Success Rate**: 1/1 (100%)
- **Execution Time**: Avg = 274ms, Min = 274ms, Max = 274ms
- **Iterations**: Avg = 2313, Min = 2313, Max = 2313
- **Solution Depth**: Avg = 31,0, Min = 31, Max = 31

**Individual Runs:**
| Run | Time (ms) | Iterations | Found Solution | Solution Depth |
|-----|-----------|------------|----------------|----------------|
| 1 | 274 | 2313 | Yes | 31 |

#### Without Hash Verification
- **Success Rate**: 1/1 (100%)
- **Execution Time**: Avg = 334ms, Min = 334ms, Max = 334ms
- **Iterations**: Avg = 2313, Min = 2313, Max = 2313
- **Solution Depth**: Avg = 31,0, Min = 31, Max = 31

**Individual Runs:**
| Run | Time (ms) | Iterations | Found Solution | Solution Depth |
|-----|-----------|------------|----------------|----------------|
| 1 | 334 | 2313 | Yes | 31 |

#### Analysis
- Hash verification made the algorithm **60ms faster** (18% improvement).
- With hash verification, the algorithm explored **-0 more states** (-0% increase).
- Success rate remained the same with hash verification.

### Breadth First Search
#### With Hash Verification
- **Success Rate**: 0/1 (0%)
- **Execution Time**: Avg = 131327ms, Min = 131327ms, Max = 131327ms
- **Iterations**: Avg = 1000000, Min = 1000000, Max = 1000000

**Individual Runs:**
| Run | Time (ms) | Iterations | Found Solution | Solution Depth |
|-----|-----------|------------|----------------|----------------|
| 1 | 131327 | 1000000 | No | N/A |

#### Without Hash Verification
- **Success Rate**: 0/1 (0%)
- **Execution Time**: Avg = 119188ms, Min = 119188ms, Max = 119188ms
- **Iterations**: Avg = 1000000, Min = 1000000, Max = 1000000

**Individual Runs:**
| Run | Time (ms) | Iterations | Found Solution | Solution Depth |
|-----|-----------|------------|----------------|----------------|
| 1 | 119188 | 1000000 | No | N/A |

#### Analysis
- Hash verification made the algorithm **12139ms slower** (9% degradation).
- With hash verification, the algorithm explored **-0 more states** (-0% increase).
- Success rate remained the same with hash verification.

## Conclusion
Based on the performance analysis across 7 different search algorithms:
- **2** algorithms performed better **with** hash verification
- **5** algorithms performed better **without** hash verification

### Key Findings
1. Hash verification generally reduces the number of states explored, which can lead to performance improvements in algorithms that explore many redundant states.
2. For some algorithms (especially depth-limited ones), the overhead of hash computation and checking may outweigh the benefits of avoiding redundant states.
3. The benefit of hash verification tends to increase with longer-running searches where more symmetric positions are encountered.
