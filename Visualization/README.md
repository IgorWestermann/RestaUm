# Algorithm Visualization with Graphviz

This module provides tools to visualize solution trees and paths for the Peg Solitaire algorithms.

## Features

- Generates DOT files for full search trees
- Creates DOT files specifically for solution paths
- Limits tree output for large searches to prevent excessive file sizes
- Visualizes board states in a text-based format within nodes

## Requirements

- [Graphviz](https://graphviz.org/download/) must be installed to convert DOT files to images
- Make sure `dot` is in your system PATH

## How to Use

1. **Run the algorithms** - When you run any of the search algorithms with the visualization enabled, it will generate DOT files in the `reports` directory
2. **Process the DOT files** - Use the provided scripts to convert DOT files to images:
   - On Windows: Run `generate_graphs.bat`
   - On Linux/macOS: Run `./generate_graphs.sh` (make it executable first with `chmod +x generate_graphs.sh`)
3. **View the images** - The generated PNG images will be in the `reports/images` directory

## File Naming Convention

- `tree_[AlgorithmName]_[DateTime].dot` - Full search tree
- `path_[AlgorithmName]_[DateTime].dot` - Solution path only

## Visualization Details

### Nodes

Each node represents a board state and includes:

- Number of remaining pegs
- Level in the search tree (depth)
- Text representation of the board

### Edges

- Regular tree edges are shown in gray
- Solution path edges are highlighted in red with thicker lines

## For Large Trees

When the search tree exceeds 5000 nodes, only the solution path will be generated to avoid performance issues. The full tree visualization is still available for smaller searches.

## Customization

You can modify `GraphvizGenerator.cs` to customize the visualization:

- Change node colors and styles
- Adjust the maximum number of nodes to render
- Modify edge appearance
- Add additional information to nodes
