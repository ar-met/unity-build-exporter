# Unity Build Exporter

Provides an execute method that handles versioning dependent on passed commandline arguments. These are custom commandline arguments that are parsed by this package in addition to all other commandline arguments that Unity supports per default when run in batchmode. See here for a [full list of commandline arguments that are supported by Unity](https://docs.unity3d.com/Manual/CommandLineArguments.html).

This package will work for all supported Unity platforms, but was developed with _Android_ and _iOS_ as its main platforms in mind. Note that this package integrates nicely with the [_fastlane plugin unity exporter_](https://github.com/ar-met/fastlane-plugin-unity-exporter) and therefore per default has `Export Project` (see [Unity Editor Build Settings](https://docs.unity3d.com/Manual/android-BuildProcess.html)) ticked and exports _Android_ projects instead of directly creating _APK_ files.


## Custom commandline arguments

`newVersion`: Expects a new version that conforms to [semantic versioning](https://semver.org/) or one of the keywords `major`, `minor` or `patch`. Either the provided semantic version will be set as the new version of the project or the existing semantic version will receive a `major`, `minor` or `patch` bump.

`newVersionCode`: Expects a new version code or the keyword `increment`. Note that a new version must be a non-negative number. Now either the provided version code will be set as the new version code of the project or the existing version code will be incremented. The version code is shared across all platforms, which means that _Android_'s _versionCode_ and _iOS_' _buildNumber_ share the same value.

`exportPath`: The destination path of the exported Unity project. If no `exportPath` is defined, a default path near the _Assets_ directory will be used.


## Usage

Note that the above described custom commandline arguments will only work in conjunction with the execute method of this package. This method handles the parsing and actual versioning. Therefore when invoking Unity via batchmode the following execute method must be defined: `-executeMethod armet.BuildExporter.BuildUtility.CreateBuild`


    {path-to-unity} -batchmode -nographics -quit -executeMethod armet.BuildExporter.BuildUtility.CreateBuild -newVersion 1.2.3 -newVersionCode 0

    {path-to-unity} -batchmode -nographics -quit -executeMethod armet.BuildExporter.BuildUtility.CreateBuild -newVersion major -newVersionCode 1

    {path-to-unity} -batchmode -nographics -quit -executeMethod armet.BuildExporter.BuildUtility.CreateBuild -newVersionCode increment

    {path-to-unity} -batchmode -nographics -quit -executeMethod armet.BuildExporter.BuildUtility.CreateBuild -newVersion 4.5.6 -newVersionCode 0 -exportPath some-build-directory


## Further remarks

**Don't forget to commit the updated version and version code!** 

The above mentioned [_fastlane plugin unity exporter_](https://github.com/ar-met/fastlane-plugin-unity-exporter) has an action for this.
