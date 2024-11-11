# in3D Avatars Unity SDK (runtime models loading)
![in3D logo](https://in3d.io/wp-content/uploads/2020/05/logo.png)

## Description
SDK contains in3D Api interface. This SDK gives access to in3D avatar models, textured, fully rigged, prepared for animations.
- We use async, so Unity under 2017 is not supported.
- We do not use third party plugins for json deserialization or web requests. So platform support depends on Unity version.

## Features
- Login in3d user
- Get list of users avatars
- Get urls for avatars models
- Downloading of avatars glb and fbx models in editor
- Downloading of avatars glb models in runtime
- Downloading of models previews in runtime

## Dependencies
- [glTFast](https://github.com/atteneder/glTFast)
- [in3D Avatars Sdk (minimal)](https://assetstore.unity.com/packages/slug/205111)

## Installation
Please follow the instructions:
### Install via Unity Package Manager

1. Open **Edit->Project Settings->Package Manager**
2. Add a new Scoped registry for glTFast
    ```
      Name: package.openupm.com
      URL: https://package.openupm.com,
      Scope(s): com.atteneder
   ```
3. Add a new Scoped Registry for SDK
    ```
    Name: unity.in3d.io
    URL: https://unity.in3d.io
    Scope(s): com.in3d.sdk
    ```
4. Click **Save** (or **Apply**)
5. Install `com.in3d.package.loaders` package in PackageManager / My Registries

**Alternatively**, merge the snippet to **Packages/manifest.json**
```json
{
  "scopedRegistries": [
    {
      "name": "unity.in3d.io",
      "url": "https://unity.in3d.io",
      "scopes": [
        "com.in3d.sdk"
      ]
    },
    {
      "name": "OpenUPM",
      "url": "https://package.openupm.com",
      "scopes": [
        "com.atteneder"
      ]
    }
  ],
  "dependencies": {
    "com.in3d.sdk.minimal": "last",
    "com.in3d.sdk.loaders": "last",
    "com.atteneder.gltfast": "4.7.0"
  }
}
```

### Install via Unity Asset Store

Just install `unitypackage` from [package page]() in unity asset store. 
All dependencies will be resolved automatically.

## How to use
Create user credentials scriptable object via unity **Project window -> Create -> in3D -> Access Key Credentials**.
In created object set up **Access Key** field with key from in3d mobile application and set server settings (use settings shipped with this package).
Another way to create user credentials is via script:
```c#
ScriptableObject.CreateInstance<AccessKeyCredentials>().Init("access key", ServerSettings.Default);
```

Use `AccessKeyCredentials.Login()` function to get `IUser` object. 
Via this object you can access avatars.
Fetch avatars for user from server:
```c#
await user.UpdateAvatarsListAsync();
```

Add empty `GameObject` to scene and add `GlbLoader` component on it. 
Avatar's model will be loaded as child of this game object, other children will be destroyed.

```c#
GlbLoader.LoadAvatarAsync(user.Avatars.First());
```

Also you can load avatars preview as `Texture2D`:
```c#
Texture2D texture = await user.Avatars.First()[ModelFormat.Glb].LoadPreviewAsync();
```

Full example:
```c#
public async Task LoadFirstAvatarsAsync(string userAccessKey, GlbLoader loader)
{
    var creds = AccessKeyCredentials.Create(userAccessKey, ServerSettings.Default);

    IUser user;
    try {
        user = await creds.LoginAsync();
    }
    catch (InvalidCredentialsException) {
        Debug.LogError("Cannot login user due to invalid credentials");
        return;
    }

    await user.UpdateAvatarsListAsync();

    if (user.Avatars.Count == 0) {
        Debug.LogWarning("User has no avatars");
        return;
    }
    
    AvatarInfo avatar = user.Avatars.First();
    Texture2D preview = await avatar[ModelFormat.Glb].LoadPreviewAsync();
    await loader.LoadAvatarAsync(avatar);
}
```
Please refer to Readme file of `com.in3d.sdk.minimal` package for more use cases.

## Samples
This package contains 2 Sample scenes where you can see the whole process:
- user login
- previews loading
- selecting avatar by preview
- model loading
- animated model

## Support
- E-mail: hello@in3d.io
- Discord: https://discord.gg/bRzFujsHH9

## Project status
On going
