fn main() {
    /*
       Note: Tauri v2 usually handles manifest injection internally or via config,
       but for specific execution levels, we might need embed-resource if tauri-build doesn't expose it.
       However, tauri-build 2.0+ supports .windows_attributes() on the builder.
    */
    /*
    // Standard Tauri v2 way if method is available:
    let mut windows = tauri_build::WindowsAttributes::new();
    windows = windows.app_manifest(include_str!("app.manifest"));
    tauri_build::try_build(network_build_attributes).expect("failed to run tauri-build");
    */

    // Since we want to ensure it works without complex dependency checks first,
    // let's try the simple Builder approach if available, or just let Tauri build and see
    // if we need `embed-resource`. Given Tauri v2, let's use the Attributes.

    let attrs = tauri_build::Attributes::new().windows_attributes(
        tauri_build::WindowsAttributes::new().app_manifest(include_str!("app.manifest")),
    );
    tauri_build::try_build(attrs).expect("failed to run build script");
}
