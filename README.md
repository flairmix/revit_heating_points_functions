# Fill_ADSK_Parameters

Внешние команды для Revit 2025, которые заполняют ADSK-параметры для спецификаций ИТП/тепловых пунктов.

## Команды

`ADSK Pipes Length`

Пересчитывает `ADSK_Количество` для труб по длине в метрах. `ADSK_Наименование` заполняется только если параметр пустой.

`ADSK Pipe Insulation`

Пересчитывает `ADSK_Количество` для изоляции по длине в метрах. `ADSK_Наименование` заполняется только если параметр пустой.

`ADSK Grouping`

Заполняет `ADSK_Группирование` по категории элемента.

`ADSK Position Fill`

Заполняет `ADSK_Позиция` по каталогу правил и нумерует разные сочетания наименования и марки внутри одной базовой позиции: например, `18.1`, `18.2`. При повторном запуске совпавшие с правилами позиции пересчитываются.

Правила позиций лежат в CSV:

```text
Fill_ADSK_Parameters\position_rules.csv
```

Формат строк:

```csv
group;pattern;base_position
Арматура;Кран шаровой;18
```

Файл копируется рядом с DLL при сборке. После сборки можно менять CSV рядом с `Fill_ADSK_Parameters.dll` без пересборки проекта. Команда перечитает CSV при следующем запуске, если файл был изменён.

`ADSK Copy Comments Inside`

Копирует `Comments` из родительского семейства во вложенные семейства только если `Comments` у вложенного семейства пустой.

## Сборка

```powershell
dotnet build Fill_ADSK_Parameters.sln -c Release
```

Если Revit уже загрузил `bin\Release\Fill_ADSK_Parameters.dll`, сборка может не перезаписать DLL. Закройте Revit или соберите в отдельную папку для проверки:

```powershell
dotnet build Fill_ADSK_Parameters.sln -c Release /p:OutputPath=bin\CodexVerify\
```

## Манифест Revit 2025

Используется только актуальный манифест:

```text
Fill_ADSK_Parameters\Fill_ADSK_Parameters_2025.addin
```

Путь к DLL внутри манифеста сейчас настроен на:

```text
C:\Users\mid\source\repos\revit_heating_points_functions\Fill_ADSK_Parameters\bin\Release\Fill_ADSK_Parameters.dll
```

Для подключения команды скопируйте `.addin` в:

```text
%APPDATA%\Autodesk\Revit\Addins\2025
```
