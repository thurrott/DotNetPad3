# .NETpad 3.0 for Windows 11

This is a major update to [the original Windows Presentation Foundation (WPF) version of .NETpad](https://github.com/thurrott/NotepadWPF) that adds support for Windows 11 theming per the changes Microsoft introduced in .NET 9 in late 2024. However, that implementation is buggy and unreliable, and .NETpad 3.0 inherits some of those problems. 

![dotnetpad-3-win11](https://github.com/user-attachments/assets/57c8f258-9487-4970-8af6-91c64c2006a4)

This version of the app supports a single document per app instance as before, but a future version will add support for tabs and multiple documents, and .NETpad 3.0 includes some code related to that coming shift.

## What's new

**Windows 11 theming support.** This version of the app looks and feels native in Windows 11. In fact, it requires Windows 11, though it wouldn't be too difficult to adapt it to look and run well in Windows 10 too. It supports Light, Dark, and System theme modes and most Windows 11 controls (WPF doesn't support the native ToggleSwitch control, so I use a ToggleButton instead). And it sports a nice translucent look where appropriate.

![dotnetpad-3-app-themes](https://github.com/user-attachments/assets/25bda5cf-83dc-4e9b-9e5d-20b0451c6a11)

**Toggle app theme.** Like other modern Windows 11 apps, .NETpad 3.0 provides a settings interface so the user can switch between Light, Dark, and System theme modes. There is also a toggle button in the main app window for switching between these modes, with a tooltip that communicates the current app theme. Note that the app will crash if an Expander control is open or has been opened: This is a bug in .NET 9 that Microsoft will fix. So it will work fine in the near future.

![app-theme](https://github.com/user-attachments/assets/22b497d6-aed9-4676-b52b-2973c8615f4f)

**Accent color support in custom dialog boxes.** The custom dialogs correctly adopt the system accent color for the default button in the new custom dialog boxes. (See below.)

![accent-color](https://github.com/user-attachments/assets/32daebe1-4101-400a-b166-04344ff761f4)

**Customized title bar.** In an interim step toward the tabbed-based UI coming in the next version of the app, .NETpad 3.0 uses a fully-customized title bar area at the top of the window with an app logo, the current document name, a draggable and double-clickable area, and the standard Close, Maximize/Restore, and Close window buttons. For now, you have to type "Alt + Space" to display the system window menu, but a future version will include a right-click capability. This customization also precludes the use of the Snap layouts pop-up that's automatically provided by Windows 11 to apps. I will try to add this feature back in a future version of the app too.

![custom-titlebar](https://github.com/user-attachments/assets/df6fe221-fe17-4fe9-b9bb-edb8d46392b7)

**DocumentTab for document-related state management.** In an interim step toward the tabbed-based UI coming in the next version of the app, .NETpad 3.0 has been restructured to use a new DocumentTab class that provides document (and tab) state management. For now, the app supports only a single document, so there is only a single DocumentTab object, but the current code will ease the transition to multiple tabs (and documents) in the future. 

**New Settings interface.** .NETpad provides a new Settings interface that takes over the entire app window and mimics the version found in Notepad in Windows 11.

![settings](https://github.com/user-attachments/assets/0785eee9-fd06-4d2a-a9e3-487c4638ed1a)

**New Spell check feature with toggle.** You can toggle the system Spell check feature on/off and the app will save your preferences to settings and retain it between app launches. WPF doesn't provide native access to the related Autocorrect feature in Windows 11, but if that is added in the future, I will update the app.

**New custom dialogs.** I created a new custom dialog template for .NETpad so that sub-windows like Auto save (which looks different for enable and disable), Confirmation, and Go to line look both native and beautiful. These windows support a nice translucency effect that I think makes them look nicer than the similar dialogs found in Notepad. (Note that there's no way to customize File save, save as, or open this way.) For now the app still uses a few standard (and thus non-styled) MessageBox dialogs, but I plan to create a custom MessageBox in the next version to address that.

![confirmation-dialogs](https://github.com/user-attachments/assets/93d39be1-3716-4c57-9bce-f162c2d0c062)

**New Find/Replace panes.** The interesting new Find and Find/Replace pop-ups in Notepad are not available in WPF, so I implemented these functions in a pop-up panel that appears as needed between the menu bar and the text box. This is a non-standard user interface, but I tried to make it look as native and natural as possible. If WPF is ever updated to support more native Windows 11 controls, I will update the app. 

![find-replace](https://github.com/user-attachments/assets/04f0640c-d8ac-4513-8679-0239c47452cc)

**Fit and finish: Menu alignment.** On touch-based systems, the File, Edit, and View menus would open to the left of the mouse cursor instead of to the right like God intended. There's probably a better way to handle this, but for now, I simply found a method that will force the menus to appear in the correct alignment, to the right of the mouse cursor. (This used to be a feature you could configure in Windows 11, but I can't find it on Surface Laptop 7 and other touch-capable laptops.)

![menu-alignment](https://github.com/user-attachments/assets/70942736-9078-4ede-a20b-c6b2c2f3cbd1)

**Fit and finish: Fixed menu checkmark weirdness.** Thanks to its buggy new support for Windows 11 theming, using a checkable menu item means that the other menu items in the same menu are all visually offset, and that's true regardless of whether that item is checked or not. To workaround this, I added an empty image to the left of the other menu items that matches the width of the system checkmark. So these menu items all line up correctly now, as they do in Notepad. 

![menu-checkmark-offset](https://github.com/user-attachments/assets/99b0f8a5-9912-46f3-bd41-a349b8fbcc05)

**Fit and finish: Double-click word count to toggle with character count.** .NETpad has always supported a word count display in the status bar, but in version 3.0, you can double-click this display to toggle it between word and character counts. There's a new tooltip describing this feature as well.

![word-char-count](https://github.com/user-attachments/assets/1d796be7-5938-4655-ae5a-a3a6287086a1)

**Fit and finish: New "unsaved" indicator.** As with Notepad, .NETpad previously denoted an unsaved document by displaying a "* " (asterisk and a space) after its name in the title bar. But Notepad now displays a "●" symbol where the Close tab button usually displays instead. As an interim step towards the coming tabbed-based UI, .NETpad now displays a "▪ " (square bullet and a space) before the name (which gets truncated if it's too long).

![unsaved-indicator](https://github.com/user-attachments/assets/f09900c9-2f5b-45a5-99d0-7aecf5b633e4)

**Removed: App scaling.** Though it was an interesting feature, I ended up removing .NETpad's app scaling capabilities in this version.

**Bug fixes.** Though the code is still amateurish, I spent a lot of time on improving the overall quality. Part of this was to make the code a bit more modular. But there are also many bug fixes. I didn't keep good track of this, but I fixed various logic issues related to Auto save (mostly timer-related, to prevent multiple Save as dialogs from appearing), Save and Save as, app close (which was causing a crash in certain circumstances), and more. I'm sure there are still plenty of bugs: I'm not a professional developer.

## Known issues

.NETpad 3.0 for Windows 11 has plenty of problems, some self-inflicted and some triggered by Microsoft's buggy implementation of Windows 11 theming in WPF in .NET 9. Some of the key issues I'm aware of include:

**App theme customization can crash the app.** If you open an Expander control (there are two in the Settings interface) and then try to change the app theme, .NETpad will crash. Microsoft is aware of this issue, which is in WPF/.NET 9 and not this app, and has fixed it internally. So this serious issue will be fixed when Microsoft releases an updated version of .NET 9 in stable. (I left the UIs to change the theme in the app in anticipation of this.)

**No right-click window menu.** Because I customized the app title bar area, you can't right-click the app icon in the upper-left of the window to display the system window menu. (Typing Alt + Space still works, however.) I will add this feature back in a future version of the app.

**No Snap layouts.** Because I customized the app title bar area, you won't see a pop-up Snap layouts grid when you hover the mouse cursor over the Maximize/Restore window button. There are complex solutions to this problem, and perhaps Microsoft will fix this issue in the future. Either way, I hope to fix this issue in a future version of the app.

**Document actions work when viewing settings.** When you view the Settings interface, normal file operations (like typing Ctrl + O to display a File open dialog) still work. That shouldn't be the case, obviously, so I will investigate ways to solve this issue.

**Custom dialog boxes are resizable.** I really like the look and feel of the custom dialogs I created for .NETpad 3.0, but there is one major issue there, and it's tied to WPF's buggy support for Windows 11 theming: You can grab the border of these windows and drag to resize them. Obviously, the custom dialogs should not be resizable. But the only way to style these windows correctly requires this. I will investigate a workaround for a future version of the app. 

**Selection rectangles.** Microsoft introduced a bizarre new usability feature in Windows 11 in 2024 that displays a garish black selection rectangle around the default control in a newly opened window. (You can see this in File Explorer, for example: The "Quick access" control is selected by default.) I'm not aware of a way to disable this ridiculous feature, which appears intermittently. But I will try to figure that out for a future version of the app (and hope that Microsoft adds a way to prevent this to Windows 11).

![selection-rectangle](https://github.com/user-attachments/assets/ebbca0df-0d4c-4713-90d5-4c26e39110e7)

I'm sure there's more, including some I know about and probably many I do not. 
