# Project README

## Overview

This repository contains two .NET applications: **SimulateWorkReductionWithThreads** and **ProducerConsumerSimulation**. Both projects are designed to demonstrate multithreading concepts and the producer-consumer problem using .NET 8.0.

## Table of Contents

- [Getting Started](#getting-started)
- [Project Structure](#project-structure)
- [Usage](#usage)
- [Contributing](#contributing)
- [License](#license)

## Getting Started

To get started with the projects, ensure you have the following prerequisites installed:

- [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0)
- A code editor such as [Visual Studio](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/)

### Cloning the Repository

Clone the repository to your local machine using the following command:

```bash
git clone https://github.com/yourusername/yourrepository.git
```

### Navigating to the Project Directory

Change to the project directory:

```bash
cd dotnetPractice
```

## Project Structure

The repository contains the following structure:

```
dotnetPractice/
├── ProducerConsumerSimulation/
│   ├── ProducerConsumerSimulation.csproj
│   ├── Program.cs
│   └── obj/
├── SimulateWorkReductionWithThreads/
│   ├── SimulateWorkReductionWithThreads.csproj
│   ├── Program.cs
│   └── obj/
└── dotnetPractice.sln
```

- **ProducerConsumerSimulation/**: Contains the implementation of the producer-consumer problem.
- **SimulateWorkReductionWithThreads/**: Contains the implementation of a simulation that reduces work using threads.
- **dotnetPractice.sln**: The solution file that includes both projects.

## Usage

To run either project, navigate to the respective project directory and use the following command:

```bash
dotnet run
```

### Running ProducerConsumerSimulation

Navigate to the `ProducerConsumerSimulation` directory:

```bash
cd ProducerConsumerSimulation
dotnet run
```

### Running SimulateWorkReductionWithThreads

Navigate to the `SimulateWorkReductionWithThreads` directory:

```bash
cd SimulateWorkReductionWithThreads
dotnet run
```

## Contributing

Contributions are welcome! If you have suggestions for improvements or new features, please fork the repository and submit a pull request.

1. Fork the repository.
2. Create a new branch (`git checkout -b feature-branch`).
3. Make your changes and commit them (`git commit -m 'Add new feature'`).
4. Push to the branch (`git push origin feature-branch`).
5. Open a pull request.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
