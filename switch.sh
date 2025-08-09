#!/bin/bash

# Check if the script is run as root
if [ "$(id -u)" -ne 0 ]; then
    echo "This script needs to be run as root or with sudo."
    exit 1
fi

# Prompt the user to select a Java version
echo "Select a Java version to switch to:"
echo "1) Java 8"
echo "2) Java 11"
echo "3) Java 17"
echo "4) Java 21"

read -p "Enter your choice (1/2/3/4): " choice

# Set the Java version based on the user's input
case $choice in
    1)
        echo "Switching to Java 8..."
        sudo update-alternatives --set java /opt/jdk8u462-b08/bin/java
        sudo update-alternatives --set javac /opt/jdk8u462-b08/bin/javac
        ;;
    2)
        echo "Switching to Java 11..."
        sudo update-alternatives --set java /opt/jdk-11.0.28+6/bin/java
        sudo update-alternatives --set javac /opt/jdk-11.0.28+6/bin/javac
        ;;
    3)
        echo "Switching to Java 17..."
        sudo update-alternatives --set java /opt/jdk-17.0.16+8/bin/java
        sudo update-alternatives --set javac /opt/jdk-17.0.16+8/bin/javac
        ;;
    4)
        echo "Switching to Java 21..."
        sudo update-alternatives --set java /opt/jdk-21.0.8+9/bin/java
        sudo update-alternatives --set javac /opt/jdk-21.0.8+9/bin/javac
        ;;
    *)
        echo "Invalid choice. Please run the script again and choose a valid option."
        exit 1
        ;;
esac

# Show the currently active Java version
echo "Java version after switch:"
java -version
