# Async Load Texture

Load texture for images completly asynchronyously.

# Dependency
1) UniTask - see https://github.com/Cysharp/UniTask
2) MRTK (Mixed Reality Toolkit by Microsoft) - see https://github.com/microsoft/MixedRealityToolkit-Unity

# Usage

1) Create instance of DiskCache (custom or one of the provided).
2) Create instance of LoadImagesAsync
3) Add DiskCache to LoadImagesAsync
4) Use LoadImagesAsync(url,headers, cancellationToken)

# Important

Be sure to check out if the target platform supports given cache folder : https://docs.unity3d.com/ScriptReference/Application-persistentDataPath.html


# TODO:
Benchmark - UI + load Images from DiskCache vs UI + load images from PlayerPrefs (base 64)