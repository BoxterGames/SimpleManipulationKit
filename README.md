# Simple Manipulation Kit

Пакет для базового выбора и перемещения объектов в Unity: одиночный и множественный выбор, drag для UI и 3D, выделение рамкой и подсветка выбранного элемента.

Текущая версия пакета рассчитана на **Unity 6.0** (`6000.0`). Ввод мыши реализован через старый `UnityEngine.Input`.

## Установка

В Unity откройте **Window → Package Manager**, нажмите **+ → Add package from git URL…** и укажите:

```text
https://github.com/BoxterGames/SimpleManipulationKit.git
```

В **Project Settings → Player → Other Settings → Active Input Handling** выберите **Input Manager (Old)** или **Both**. Перезапустите редактор, если Unity предложит это сделать.

## Быстрый обзор

- `ISelectable` помечает компонент как выбираемый.
- `IDraggable` помечает выбираемый компонент как перемещаемый.
- `ManipulationObject` — готовая реализация `ISelectable` и `IDraggable`.
- `ManipulationController` объединяет выбор, перетаскивание и участие в выделении рамкой для UI.
- `ManipulationController3D` делает то же для 3D-объекта с `Collider`.
- `InteractionContext.Selection` — общее состояние выбора; на `SelectionModel.OnUpdateSelected` можно подписать свою подсветку или логику.
- `MarqueeController` запускает выделение рамкой, а `MarqueeView` отображает рамку. Сам `MarqueeController` не создаёт визуальный элемент.

Контроллеры лежат в пространстве имён `SimpleManipulationKit.Internal`, хотя их можно добавлять на GameObject через Inspector.

## Минимальная сцена: UI

1. Создайте `EventSystem`. На Canvas должны быть `GraphicRaycaster` и UI-элементы с включённым **Raycast Target**.
2. Добавьте в сцену prefab `Runtime/Prefabs/ManipulationGlobalController.prefab`. В нём находятся `MarqueeController` и `SelectionResetController`: первый управляет рамкой, второй снимает выбор по `Escape` или клику в пустое место.
3. Создайте `Canvas` в режиме **Screen Space - Overlay**.
4. Для каждого перемещаемого элемента создайте UI-объект с `RectTransform` и `Image` (или другой raycastable `Graphic`). Добавьте `ManipulationObject` и `ManipulationController` на тот же объект. Поле `View` обычно заполнится автоматически; если нет — перетащите в него компонент `ManipulationObject`.
5. Для видимой рамки создайте UI-объект с `Image` внутри Canvas и добавьте `MarqueeView`. Настройте цвет Image, например полупрозрачную заливку или спрайт с обводкой. `MarqueeView` сам меняет размер и позицию элемента; при неактивной рамке его ширина и высота равны нулю.

ЛКМ по элементу выбирает и начинает перемещение. Зажмите **Shift**, чтобы добавить элементы к выбору, или **Ctrl**, чтобы переключить их выбор. Перетаскивание по пустой области создаёт рамку.

**Важно для Canvas:** не размещайте поверх всей рабочей области raycastable `Image` или другую `Graphic` с включённым `Raycast Target`. `MarqueeController` начинает рамку только когда указатель не находится над UI; полноэкранный фон с raycast блокирует и рамку, и снятие выбора кликом по фону. Для фона отключите `Raycast Target`.

Если нужен только клик-выбор UI без перемещения, вместо `ManipulationController` используйте `SelectionController`. Не добавляйте оба контроллера на один элемент без особой причины: оба обрабатывают нажатие и могут менять выбор.

## Минимальная сцена: 3D

1. Добавьте в сцену камеру с тегом **MainCamera**.
2. Добавьте prefab `Runtime/Prefabs/ManipulationGlobalController.prefab`.
3. Создайте 3D-объект с `Collider` (например, Cube), добавьте на него `ManipulationObject` и `ManipulationController3D`.
4. Для базового перемещения по горизонтальной плоскости оставьте `Space Converter = XoZSpaceConverter`. Объект перемещается по плоскости XZ родителя (или самого объекта, если родителя нет).

На объект можно добавить свой компонент, реализующий `ISelectable`/`IDraggable`, вместо `ManipulationObject`. Камера и коллайдер нужны для обнаружения клика; контроллер 3D требует `Collider` автоматически.

## Свой выбираемый или перемещаемый объект

Контроллер ищет компонент, реализующий нужный интерфейс, на текущем GameObject и среди его дочерних объектов. Можно использовать готовый `ManipulationObject` либо реализовать интерфейсы в своём `MonoBehaviour`:

```csharp
using SimpleManipulationKit;
using UnityEngine;

public sealed class SnapOnDrop : MonoBehaviour, IDraggable, IDraggableEnd
{
    public void OnDragEnd(Vector3 localPosition)
    {
        transform.localPosition = new Vector3(
            Mathf.Round(localPosition.x),
            Mathf.Round(localPosition.y),
            localPosition.z);
    }
}
```

`IDraggable` наследует `ISelectable`, поэтому отдельная реализация `ISelectable` здесь не нужна. `DragCalculator` напрямую меняет `transform.localPosition`; дополнительные интерфейсы позволяют реагировать на начало, обновление и конец перетаскивания:

- `IDraggableAvailable.CanDrag()` — разрешить или запретить перетаскивание в данный момент.
- `IDraggableStart.OnDragStart(Vector3 localPosition)` — начало.
- `IDraggableUpdate.OnDragUpdate(Vector3 localPosition)` — обновление позиции.
- `IDraggableEnd.OnDragEnd(Vector3 localPosition)` — завершение.

Позиция в callbacks — локальная позиция объекта. Для сохранения позиции при перетаскивании группа берёт точку захвата и смещение каждого выбранного объекта.

Чтобы отобразить выбор своей логикой, подпишитесь на `InteractionContext.Selection.OnUpdateSelected` и проверьте `InteractionContext.Selection.Contains(mySelectable)`. Подписку снимайте при выключении компонента.

## Свои калькуляторы выбора

`ISelectionCalculator` задаёт правило выбора одного объекта. Пакет поставляет два варианта:

- `MultiSelection` (по умолчанию): обычный клик заменяет выбор, **Shift** добавляет, **Ctrl** переключает объект. Если уже выбранный перемещаемый объект перетаскивают, текущая группа выбора сохраняется.
- `SimpleSelection`: каждый клик заменяет выбор одним объектом.

Чтобы добавить собственное правило, создайте сериализуемый класс, реализующий `ISelectionCalculator`:

```csharp
using System;
using SimpleManipulationKit;
using SimpleManipulationKit.Internal;

[Serializable]
public sealed class AddOnlySelection : ISelectionCalculator
{
    public void Select(ISelectable selectable)
    {
        InteractionContext.Selection.Add(selectable);
    }
}
```

После компиляции выберите `AddOnlySelection` в выпадающем поле **Selection Calculator** на `SelectionController`, `SelectionController3D`, `ManipulationController`, `ManipulationController3D`, `DragController` или `DragController3D`. Поле использует `[SerializeReference]`, а список типов строится из конкретных реализаций интерфейса. Класс должен быть доступен Unity как обычный сериализуемый тип: не абстрактный, не generic и с конструктором без параметров.

Калькулятор применяется при выборе кликом и при старте drag. Выделение рамкой сейчас отдельно обрабатывается `MultiSelection` и учитывает Shift/Ctrl независимо от выбранного `ISelectionCalculator`. Сам `DragCalculator` не подключается через интерфейс для замены алгоритма движения; для ограничений и дополнительного поведения используйте `IDraggableAvailable` и drag callbacks.

## Свои преобразователи пространства

`ISpaceConverter` отвечает за преобразование координат указателя, размер/центр рамки и проверку попадания объекта в рамку. Встроенные реализации:

- `ScreenSpaceConverter` — UI под `RectTransform`, использует координаты Canvas.
- `XoZSpaceConverter` — плоскость XZ; значение по умолчанию для `ManipulationController3D`.
- `XYSpaceConverter` — плоскость XY.

Можно реализовать `ISpaceConverter` и выбрать его в поле **Space Converter** контроллера или в `MarqueeView`. Это нужно, например, для перетаскивания по своей плоскости, проекции или системе координат. Для 3D-преобразователей нужна активная камера с тегом `MainCamera`; при нескольких камерах назначайте `Interaction Camera` там, где поле доступно.

Тип должен быть `[Serializable]`, конкретным и иметь конструктор без параметров, поскольку Inspector создаёт его через `[SerializeReference]`. Нужно реализовать все методы:

```csharp
public interface ISpaceConverter
{
    Vector3 ScreenToWorldPoint(Transform reference, Vector2 screenPoint);
    Vector3 ScreenToLocalPoint(Transform reference, Vector3 screenPoint);
    Vector3 GetSize(Transform reference, Vector3 localA, Vector3 localB);
    Vector3 GetCenterPosition(Transform reference, Vector3 localA, Vector3 localB);
    bool IsIntersect(Transform reference, Vector3 screenA, Vector3 screenB);
}
```

`IsIntersect` решает, входит ли элемент в рамку; встроенные реализации проверяют позицию pivot объекта, а не пересечение рамки с полным `Collider` или границами `RectTransform`. Если нужна проверка по bounds, реализуйте её в своём преобразователе.

## Отдельные контроллеры

Если all-in-one контроллер не подходит, используйте компоненты отдельно:

- `SelectionController` и `SelectionController3D` — только выбор.
- `DragController` и `DragController3D` — только перемещение; объект должен реализовать `IDraggable`.
- `MarqueSelectController` — участие отдельного выбираемого объекта в рамке.
- `SelectionImageView` — смена цвета `Image` в зависимости от выбора.

Для UI-контроллеров требуется `EventSystem` и raycastable `Graphic`. Для 3D-контроллеров требуются коллайдер и камера; для 3D-ввода используется `OnMouseDown`.

## Модели и события

Состояние хранится в общих моделях `InteractionContext`:

- `SelectionModel`: `Add`, `Remove`, `Toggle`, `Set`, `Clear`, `Contains`, `GetSelected<T>()`; событие `OnUpdateSelected`.
- `DragModel`: события `OnDragStart`, `OnDragUpdate`, `OnDragEnd` и экранные координаты указателя `StartPosition`/`EndPosition`.
- `MarqueeModel`: события `OnMarqueeStart`, `OnMarqueeUpdate`, `OnMarqueeEnd` и состояние рамки.

События моделей удобно использовать для UI, звука, игрового поведения и интеграции с собственным кодом. Подписывайтесь и отписывайтесь в паре, чтобы избежать повторных подписок.

## Ограничения ввода

- Пакет использует `UnityEngine.Input` и левую кнопку мыши.
- UI взаимодействует через Unity EventSystem; проверьте, что `GraphicRaycaster`, `Raycast Target` и `EventSystem` настроены.
- Обнаружение 3D-клика идёт через `OnMouseDown`; объекту нужен `Collider`, а взаимодействие с UI блокируется, когда указатель над UI.
- Стандартный сброс выбора по пустому клику использует `Camera.main` и `Physics.Raycast` для 3D-объектов.
