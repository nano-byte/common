// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Storage;

[Collection(nameof(Locations))]
public class LocationsTest
{
    [Fact]
    public void InstallBaseDoesNotEndWithSlash()
    {
        Locations.InstallBase.Should().NotEndWith(Path.DirectorySeparatorChar.ToString());
    }

    [Fact]
    public void GetSaveConfigPathInPortableMode()
    {
        using var tempDir = new TemporaryDirectory("locations-test");
        using (Locations.Redirect(tempDir))
        {
            string path = Locations.GetSaveConfigPath("test-app", isFile: true, "subdir", "test.conf");
            path.Should().Be(Path.Combine(tempDir, "config", "subdir", "test.conf"));
            Directory.Exists(Path.Combine(tempDir, "config", "subdir")).Should().BeTrue();
        }
    }

    [Fact]
    public void GetSaveDataPathInPortableMode()
    {
        using var tempDir = new TemporaryDirectory("locations-test");
        using (Locations.Redirect(tempDir))
        {
            string path = Locations.GetSaveDataPath("test-app", isFile: false, "subdir", "test-data");
            path.Should().Be(Path.Combine(tempDir, "data", "subdir", "test-data"));
            Directory.Exists(path).Should().BeTrue();
        }
    }

    [Fact]
    public void GetLoadConfigPathsInPortableMode()
    {
        using var tempDir = new TemporaryDirectory("locations-test");
        string configDir = Path.Combine(tempDir, "config", "subdir");
        Directory.CreateDirectory(configDir);
        string configFile = Path.Combine(configDir, "test.conf");
        File.WriteAllText(configFile, "test");

        using (Locations.Redirect(tempDir))
        {
            var paths = Locations.GetLoadConfigPaths("test-app", isFile: true, "subdir", "test.conf");
            paths.Should().Contain(configFile);
        }
    }

    [Fact]
    public void GetLoadDataPathsInPortableMode()
    {
        using var tempDir = new TemporaryDirectory("locations-test");
        string dataDir = Path.Combine(tempDir, "data", "subdir");
        Directory.CreateDirectory(dataDir);
        string dataFile = Path.Combine(dataDir, "test-data");
        File.WriteAllText(dataFile, "test");

        using (Locations.Redirect(tempDir))
        {
            var paths = Locations.GetLoadDataPaths("test-app", isFile: true, "subdir", "test-data");
            paths.Should().Contain(dataFile);
        }
    }

    [Fact]
    public void GetCacheDirPathInPortableMode()
    {
        using var tempDir = new TemporaryDirectory("locations-test");
        using (Locations.Redirect(tempDir))
        {
            string path = Locations.GetCacheDirPath("test-app", machineWide: false, "subdir");
            path.Should().Be(Path.Combine(tempDir, "cache", "subdir"));
            Directory.Exists(path).Should().BeTrue();
        }
    }

    [Fact]
    public void GetCacheDirPathNormalMode()
    {
        string path = Locations.GetCacheDirPath("test-app", machineWide: false, "subdir");
        path.Should().Be(Path.GetFullPath(Path.Combine(Locations.UserCacheDir, "test-app", "subdir")));
        Directory.Exists(path).Should().BeTrue();
    }

    [Fact]
    public void GetInstalledFilePath()
    {
        using var tempDir = new TemporaryDirectory("locations-test");
        string testFile = Path.Combine(tempDir, "test-file");
        File.WriteAllText(testFile, "test");

        string originalInstallBase = Locations.InstallBase;
        try
        {
            Locations.OverrideInstallBase(tempDir);
            Locations.GetInstalledFilePath("test-file").Should().Be(testFile);
        }
        finally
        {
            Locations.OverrideInstallBase(originalInstallBase);
        }
    }

    [Fact]
    public void GetInstalledFilePathThrowsWhenNotFound()
    {
        Action action = () => Locations.GetInstalledFilePath("non-existent-file-" + Guid.NewGuid());
        action.Should().Throw<IOException>();
    }

    [Fact]
    public void GetSaveConfigPathNormalMode()
    {
        string path = Locations.GetSaveConfigPath("test-app", isFile: true, "subdir", "test.conf");
        path.Should().Be(Path.GetFullPath(Path.Combine(Locations.UserConfigDir, "test-app", "subdir", "test.conf")));
        Directory.Exists(Path.GetDirectoryName(path)).Should().BeTrue();
    }

    [Fact]
    public void GetSaveDataPathNormalMode()
    {
        string path = Locations.GetSaveDataPath("test-app", isFile: true, "subdir", "test-data");
        path.Should().Be(Path.GetFullPath(Path.Combine(Locations.UserDataDir, "test-app", "subdir", "test-data")));
        Directory.Exists(Path.GetDirectoryName(path)).Should().BeTrue();
    }
}
