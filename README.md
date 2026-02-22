# Zoo World

Unity project demonstrating an optimized animal spawning system with
physics-based movement, collisions, and survival mechanics.\
Predators exist on the map and can consume other animals.

[Watch Video](https://drive.google.com/file/d/1MSVopLgOSmO8ccQG7Q47ZzdVkY5CRU-U/view?usp=sharing)

------------------------------------------------------------------------

## Architecture & Optimization

The architecture is built using **VContainer (Dependency Injection)**
combined with a structured game loop approach (`ITickable`,
`IFixedTickable`) and **Object Pooling**, ensuring smooth gameplay
without unnecessary `Update()` calls.

Module communication is handled through **MessagePipe (Event Bus)**.

The system that detects when objects leave the camera frustum is
adaptive and works correctly across different screen resolutions and
aspect ratios.

A component-based approach is used without classical inheritance.\
Behavior is defined through pure C# classes and interfaces,
significantly reducing the number of `MonoBehaviour` scripts and
improving maintainability.

------------------------------------------------------------------------

## Animal Extensibility

Configuration is built on **ScriptableObjects** and **Prefab Variants**.

Pipeline for adding a new animal:

1.  Create a visual skin (model/sprite)
2.  Create a Prefab Variant
3.  Configure the Prefab Variant
4.  Create an animal configuration (ScriptableObject)
5.  Configure movement rules inside the config
6.  Add the config to the spawn list
7.  Configure spawn parameters

This approach allows the system to scale to hundreds of different
animals --- from crabs to birds.

------------------------------------------------------------------------

## UI and Model-View-Presenter

UI logic is built using the **MVP (Model-View-Presenter)** pattern:

-   **Model** -- stores and processes state
-   **View** -- passive, only responsible for rendering data
-   **Presenter** -- connects Model and View, reacts to external events

This makes it possible to redesign UI without touching business logic
and simplifies testing.

------------------------------------------------------------------------

## Applied Patterns

-   Object Pool\
-   Factory\
-   Component-Based Architecture\
-   Strategy\
-   Model-View-Presenter (MVP)\
-   Dependency Injection (VContainer)\
-   Event Bus (MessagePipe)

------------------------------------------------------------------------

## Unity Version

Developed with **Unity 6000.0.62f1**
