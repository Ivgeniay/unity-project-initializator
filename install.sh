#!/bin/bash
set -e

echo "=============================================="

if ! command -v dotnet &> /dev/null; then
    echo "Error: .NET is not installed. Please install .NET 9.0+"
    exit 1
fi

CLI_PROJECT_DIR="cll/UnityInitializer.Cli"
if [ ! -d "$CLI_PROJECT_DIR" ]; then
    echo "Error: CLI project directory not found: $CLI_PROJECT_DIR"
    exit 1
fi

echo "Building Unity Initializer CLI in Release mode..."
cd "$CLI_PROJECT_DIR"

dotnet clean --configuration Release
dotnet build --configuration Release

RELEASE_DIR="bin/Release/net9.0"
if [ ! -d "$RELEASE_DIR" ]; then
    echo "Error: Release build failed - directory not found: $RELEASE_DIR"
    exit 1
fi

if [ ! -f "$RELEASE_DIR/unity-init" ]; then
    echo "Error: unity-init executable not found in $RELEASE_DIR"
    exit 1
fi

echo "Build completed successfully!"

INSTALL_DIR="/usr/local/lib/unity-init"
echo "Creating installation directory: $INSTALL_DIR"
sudo mkdir -p "$INSTALL_DIR"

echo "Copying files to $INSTALL_DIR..."
sudo cp -r "$RELEASE_DIR"/* "$INSTALL_DIR/"

sudo chmod +x "$INSTALL_DIR/unity-init"

echo "Creating symbolic link in /usr/local/bin..."
sudo ln -sf "$INSTALL_DIR/unity-init" /usr/local/bin/unity-init

echo "Verifying installation..."
if command -v unity-init &> /dev/null; then
    echo "Installation successful!"
    echo "You can now use 'unity-init' from anywhere on your system."
    echo ""
    echo "Usage examples:"
    echo "  unity-init -l                 # List available Unity versions"
    echo "  unity-init -v 6000.1.12      # Initialize Unity project"
    echo ""
    unity-init --help
else
    echo "Installation failed - unity-init command not found in PATH"
    exit 1
fi