--[[

local buildDirectory = ".build"

local info = {
    appName = "GMTK25",
    itchUrl = "gmtk25",
    iconPath = "GMTK25/Icon.bmp",
    buildDirectory = buildDirectory,

    platformToProject =
    {
        ["macos-universal"] = "GMTK25",
        ["win-x64"] = "GMTK25",
        ["linux-x64"] = "GMTK25",
    },

    butlerChannelForPlatform = function(platform)
        return "latest-" .. platform
    end,

    buildDirectoryForPlatform = function(platform)
        return buildDirectory .. '/' .. platform
    end
}

return info

]]