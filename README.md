# Fill_ADSK_Parameters

Набор внешних команд для Revit 2025, заполняющих ADSK-параметры труб, изоляции, группировки и позиций.

## Установка манифеста Revit 2025

1. Соберите проект в конфигурации `Release`.

   ```powershell
   dotnet build Fill_ADSK_Parameters.sln -c Release
   ```

2. Скопируйте файл манифеста:

   ```text
   Fill_ADSK_Parameters\Fill_ADSK_Parameters_2025.addin
   ```

   в папку пользовательских дополнений Revit 2025:

   ```text
   %APPDATA%\Autodesk\Revit\Addins\2025
   ```

   Обычно это:

   ```text
   C:\Users\<UserName>\AppData\Roaming\Autodesk\Revit\Addins\2025
   ```

3. Проверьте путь к сборке внутри манифеста. Сейчас он настроен на:

   ```text
   C:\Users\mid\source\repos\revit_heating_points_functions\Fill_ADSK_Parameters\bin\Release\Fill_ADSK_Parameters.dll
   ```

   Если проект лежит в другой папке, измените значение тега `<Assembly>` в `.addin` на фактический путь к `Fill_ADSK_Parameters.dll`.

4. Перезапустите Revit 2025.

Альтернативная системная папка для установки дополнения для всех пользователей:

```text
C:\ProgramData\Autodesk\Revit\Addins\2025
```
