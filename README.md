# README
[GitHub repository link](https://github.com/ssktyOar/Leonid_Nairi_AdvancedProgramming_Assessment_3)
# 1. DEPLOYMENT INSTRUCTIONS
## 1) Requirements
- Target framework: **net10.0**
- Dotnet runtime & SDK versions: **10.0.6**
- Godot: **Godot_v4.4.1-stable_mono**

## 2) Server & data
- Data is located in the **Builds/Data** folder
- Data (and therefore user credentials) is stored in **".customer"** files that are actually JSON UTF-8 files
- **Client** requires **Server** to run first
- - Path to selfcontained Server app build for both Linux: **Builds/Server/linux-x64**
- - Path to selfcontained Server app build for both Windows: **Builds/Server/win-x64**
- Make sure that **Data** folder **with all it contents** is located in the same folder as Server executable i.e. **linux-x64/Data** and **win-x64/Data**
- Server does not track any manual changes to the files in the **Data** folder. If you made some changes manually i.e. bypassing Server functionality you have to restart it in order for data changes to take effect
## 3) Networking & Client
- Server & Client require free **127.0.0.1:8001** port to function
- Selfcontained Client builds are in the **Builds/Client** folders
