# Unity Project Initializer

A command-line tool for quickly initializing Unity projects with predefined templates and configurations. This tool extracts embedded Unity project templates to create a complete project structure with the correct version settings.

## What it does

Unity Project Initializer creates a complete Unity project structure in the current directory by extracting embedded ZIP archives that contain predefined Unity project templates. Each template includes the necessary folders (Assets, Packages, ProjectSettings) and configuration files required for a specific Unity version.

The tool automatically:

-   Creates the standard Unity project directory structure
-   Extracts all necessary configuration files
-   Sets up the correct Unity version metadata
-   Provides suggestions for similar versions if the requested version is not found

## Commands

### List available versions

```bash
unity-init -l
unity-init --list
```

Displays all Unity versions that are available as embedded templates.

### Initialize a Unity project

```bash
unity-init -v <version>
unity-init --version <version>
```

Creates a Unity project using the specified version template in the current directory.

Example:

```bash
unity-init -v 6000.1.12
```

### Help

```bash
unity-init -h
unity-init --help
```

Shows usage information and available commands.

## Adding new Unity project templates

To add support for new Unity versions, follow these steps:

1. Create a complete Unity project with the desired version and settings
2. Create a ZIP archive containing the following folders:
    - Assets/
    - Packages/
    - ProjectSettings/
3. Name the ZIP file using the Unity version format (e.g., `6000.1.12.zip`)
4. Place the ZIP file in the `UnityInitializer.Core/Source/` directory
5. The file will be automatically included as an embedded resource during build

The embedded resources are configured in the Core project file:

```xml
<ItemGroup>
  <EmbeddedResource Include="Source\*.zip" />
</ItemGroup>
```

## Installation

### Ubuntu/Linux

1. Clone or download the project
2. Navigate to the project root directory
3. Make the installation script executable:
    ```bash
    chmod +x install.sh
    ```
4. Run the installation script:
    ```bash
    ./install.sh
    ```

The script will:

-   Build the project in Release configuration
-   Copy all necessary files to `/usr/local/lib/unity-init/`
-   Create a symbolic link in `/usr/local/bin/unity-init`
-   Add the command to your system PATH

### Windows

1. Clone or download the project
2. Open PowerShell as Administrator
3. Navigate to the project root directory
4. Run the installation script:
    ```powershell
    .\install.ps1
    ```

For forced reinstallation:

```powershell
.\install.ps1 -Force
```

The script will:

-   Build the project in Release configuration
-   Copy all necessary files to `C:\Program Files\UnityInit\`
-   Add the directory to system PATH
-   Make the command available globally

Note: You may need to restart your command prompt or PowerShell session for PATH changes to take effect.

## Requirements

-   .NET 9.0 or later
-   Unity versions corresponding to the embedded templates

## Project Structure

```
unity-project-initializer/
├── cll/
│   ├── UnityInitializer.sln
│   ├── UnityInitializer.Cli/          # Console application
│   │   └── UnityInitializer.Cli.csproj
│   └── UnityInitializer.Core/         # Core library
│       ├── Source/                    # Embedded ZIP templates
│       │   └── 6000.1.12.zip
│       └── UnityInitializer.Core.csproj
├── install.sh                        # Linux installation script
├── install.ps1                       # Windows installation script
└── README.md
```

## Usage Examples

Initialize a Unity 6000.1.12 project in the current directory:

```bash
cd /path/to/your/project
unity-init -v 6000.1.12
```

List all available Unity versions:

```bash
unity-init -l
```

If you specify a version that doesn't exist, the tool will suggest similar available versions:

```bash
unity-init -v 6000
# Output: Unity version 6000 not found. Available versions: 6000.1.12, 6000.2.11
```
