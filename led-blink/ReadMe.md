# STM32F103 BluePill LED example

## Recommended reading
- [Bare Metal C on ARM][BareMetalConARM] by Daniels Umanovskis
- [Bare Metal C++ on RPi][BareMetalCPPonRPi] by Alex Robenko
- [Mastering STM32][MasteringSTM32] by Carmine Noviello

## Requirements
 - Intel/AMD x64 platform (Windows or Linux)
 - [Visual Studio Code][VSCode]
 - [Dot Net Core 6.0][DotNetCore60]
 - [STM32 Cube Programmer][STM32CubeProgrammer]
 - [ST Link GDB Server][STLinkServer]
 - [STM32 Cube MX][STM32CubeMX]
 - [STM32F103C8T6 Blue Pill][STM32F103BluePill]

## On Window initialize/build debug
### Prepare Utilities
 - Create sibling folder "tools" where you have cloned git project. Create subfolders:
    - tools
        - | gdb
        - | stm32-tools
 - install [Dot Net Core 6.0 SDK][DotNetCore60]
 - install [Visual Studio Code][VSCode]
 - install [STM32 Cube Programmer][STM32CubeProgrammer]
    copy everyhting from install dir to "tools\stm32-tools"
    (default install path is: C:\Program Files\STMicroelectronics\STM32Cube\STM32CubeProgrammer)
 - install [STM32 Cube MX][STM32CubeMX]
 - install [ST Link GDB Server][STLinkServer]
    copy everything from bin folder to "tools\gdb"
    (default install path is: "C:\ST\STM32CubeIDE_1.6.1\STM32CubeIDE\plugins\com.st.stm32cube.ide.mcu.externaltools.stlink-gdb-server.win32_x.x.x.xxxxxxx\tools\bin")
 - open workspace 
 - Run task "initialize" (this will download gnumake, cmake and ARM toolchain and install them in tools folder)
 - For CMake kit select: GNU ARM

 ## On Linux initialize/build debug
 - Create sibling folder "tools" where you have cloned git project. Create subfolders:
    - tools
        - | gdb
        - | stm32-tools
 - install [Dot Net Core 6.0 SDK][DotNetCore60]
 - install [Visual Studio Code][VSCode]
 - install [STM32 Cube Programmer][STM32CubeProgrammer]
   - copy everyhting from install dir to "tools\stm32-tools"
 - install [STM32 Cube MX][STM32CubeMX]
 - install [ST Link GDB Server][STLinkServer]
   - run: sudo sh st-stlink-server.xxxx-linux-amd64.install.sh
   - copy file stlink-server-x.x.x-x to "tools\gdb\ST-LINK_gdbserver"
 - open workspace 
 - Run task "initialize" (this will download gnumake, cmake and ARM toolchain and install them in tools folder)
 - For CMake kit select: GNU ARM

 ### Usage
 - connect USB ST-Link programmer
 - Run task "build debug" to build debug version (ctrl+shift+B)
 - Run task "attach to gdb" to debug the project (F5) 
   - Note: Always "build debug" before you start debuging the project.

[BareMetalConARM]: https://github.com/umanovskis/baremetal-arm
[BareMetalCPPonRPi]: https://github.com/arobenko/bare_metal_cpp
[MasteringSTM32]:https://leanpub.com/mastering-stm32
[STM32CubeProgrammer]:https://www.st.com/en/development-tools/stm32cubeprog.html
[STM32CubeMX]:https://www.st.com/en/development-tools/stm32cubemx.html
[STLinkServer]:https://www.st.com/en/development-tools/st-link-server.html
[VSCode]:https://code.visualstudio.com/
[DotNetCore60]:https://dotnet.microsoft.com/download/dotnet/6.0
[STM32F103BluePill]:https://stm32-base.org/boards/STM32F103C8T6-Blue-Pill.html

