# EmuDisplayRenderer

**EmuDisplayRenderer** is a high-performance rendering utility designed to bridge the gap between hardware emulators and modern display outputs. It focuses on providing a flexible, low-latency framework for capturing, processing, and rendering buffer data from emulated environments.

# 🚀 Features

* **Low-Latency Pipeline**: Optimized frame-buffer processing for real-time responsiveness.
* **Cross-Platform Backend**: (In Progress) Support for OpenGL and DirectX rendering pipelines.
* **Custom Shaders**: Integrated support for post-processing effects (CRT filters, scanlines, and upscaling).
* **Aspect Ratio Control**: Pixel-perfect integer scaling and dynamic aspect ratio correction.

# 🛠 Installation
**Prerequisites**:
* A C++20 compatible compiler (GCC, Clang, or MSVC)
* CMake 3.15+
* Graphics Drivers with support for OpenGL 4.5+ or Vulkan

**Building from Source**
```bash
# Clone the repository
git clone https://github.com/Arima57/EmuDisplayRenderer.git
cd EmuDisplayRenderer

# Create build directory
mkdir build && cd build

# Configure and build
cmake ..
cmake --build .
```

# 🚧 Project Roadmap (Under Construction)
The following modules are currently in active development or planned for future releases:

- Vulkan Backend: Implementation of a high-efficiency Vulkan rendering path.
- Frame Interpolation: Motion smoothing for higher refresh rate displays.
+ Plugin System: Support for 3rd party rendering hooks.
- Documentation Wiki: Detailed breakdown of internal buffer handling logic.
- GUI Overlay: ImGui-based debug overlay for performance monitoring.
