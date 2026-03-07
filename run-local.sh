#!/bin/bash

# GuestAccess Local Test Script

echo "🚀 GuestAccess - Local Test"
echo "============================"
echo ""

# Check if .NET is installed
if ! command -v dotnet &> /dev/null; then
    echo "❌ .NET SDK not found. Please install .NET 10 SDK first."
    exit 1
fi

echo "✅ .NET SDK found: $(dotnet --version)"
echo ""

# Check if database exists
if [ ! -f "app.db" ]; then
    echo "📦 Creating database..."
    dotnet ef database update
    echo ""
fi

# Build the application
echo "🔨 Building application..."
dotnet build -c Release
if [ $? -ne 0 ]; then
    echo "❌ Build failed!"
    exit 1
fi
echo "✅ Build successful"
echo ""

# Run the application
echo "🎯 Starting application..."
echo ""
echo "📍 Application will be available at:"
echo "   http://localhost:5000"
echo ""
echo "🔐 Default admin credentials:"
echo "   Email: admin@localhost"
echo "   Password: Admin123!"
echo ""
echo "⚠️  IMPORTANT: Change these credentials in production!"
echo ""
echo "Press Ctrl+C to stop the application"
echo ""
echo "================================"
echo ""

dotnet run --urls "http://localhost:5000"
