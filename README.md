# Hunting For Luck Script Showcase

## About the Game | 关于游戏

**English**

*Hunting For Luck* is a 2D side-scrolling boss fight game built around an unusual combat system where luck and probability become visible, playable resources. Instead of using conventional health as a fixed value, the game treats the player’s current combat odds as a fluctuating “probability” meter that directly affects both attack outcomes and incoming danger. The boss begins the fight with a naturally unfair advantage, so victory depends not just on execution, but on managing risk, timing, and resource conversion under pressure.

To support this idea, the player can deliberately **burn luck** to regain probability and temporarily shift momentum, creating a comeback mechanic built directly into the combat loop. As a result, the fight becomes more than a standard action encounter: every movement, attack, and resource trade is part of a larger system of probabilistic advantage and reversal. My work on this project focused on implementing the player gameplay systems, the complete two-phase Legna boss AI, and the custom Behaviour Tree framework that drives the fight.

**中文**

*Hunting For Luck* 是一款 2D 横版 Boss 战游戏，其核心特色在于将“幸运”与“概率”设计成玩家可以直接感知和操作的战斗资源。与传统将生命值视为固定数值的设计不同，本作将玩家当前的战斗胜算表现为一个动态变化的“概率”数值，并让它直接影响攻击结果与承受风险。Boss 在数值上天生占优，因此玩家想要取胜，不仅要依赖操作本身，更要在高压环境下完成风险管理、时机判断与资源转换。

围绕这一设计，玩家可以主动**燃烧幸运**来换取概率回复，并在短时间内扭转战局，使“逆转”成为战斗循环中的一部分。因此，这场战斗并不只是普通的动作对抗，而是一个围绕概率优势、资源博弈与局势反转展开的系统。我在这个项目中主要负责玩家玩法系统、Legna 的完整双阶段 Boss AI，以及驱动整场战斗的自定义 Behaviour Tree 框架。

---

## My Contributions | 我的贡献

**English**

For this project, I primarily implemented:

- The player character gameplay systems, including movement, jump, dash, melee combat, ranged combat, health, and Burn Luck
- A custom Behaviour Tree framework built from scratch in Unity/C#
- The full two-phase AI for the boss Legna
- Animation-event-driven combat timing and boss action synchronization
- Object pooling support used by combat/projectile systems

**中文**

在这个项目中，我主要负责实现：

- 玩家角色玩法系统，包括移动、跳跃、冲刺、近战、远程攻击、生命系统以及 Burn Luck 机制
- 基于 Unity / C# 从零实现的自定义 Behaviour Tree 框架
- Boss Legna 的完整双阶段 AI
- 基于动画事件驱动的战斗时序与 Boss 行为同步机制
- 用于战斗 / 投射物系统的对象池支持

---

## Repository Scope | 仓库内容说明

**English**

This repository is a script showcase of the gameplay and AI systems I implemented for *Hunting For Luck*. It does not contain the full game project, art assets, or all third-party content used during development.

**中文**

这个仓库主要用于展示我在 *Hunting For Luck* 中实现的核心玩法与 AI 脚本，不包含完整游戏工程、美术资源或开发过程中使用的全部第三方内容。

## Player Behavior Implementation | 玩家行为实现

**English**

The player controller in *Hunting For Luck* is built as a modular gameplay system centered around the `Character` class, which acts as the shared state hub for movement, jump, dash, melee combat, ranged combat, input, and health. Instead of putting all logic into one large script, each major behavior is implemented in its own component and coordinated through shared runtime flags such as `isJumping`, `isGrounded`, `isMeleeAttacking`, `isShooting`, `isDashing`, `isGettingHit`, and `isDead`. This structure made the player logic easier to extend while allowing different mechanics to interrupt or block each other in a predictable way.

Input is handled through Unity’s Input System in `InputManager`. Movement, jump, melee attack, ranged attack, dash, and the special **Burn Luck** action are all routed from input callbacks into their corresponding modules. This keeps the input layer lightweight and lets each gameplay system own its own logic. Burn Luck is especially important because it connects player input directly to the game’s central risk-reward mechanic: the player can sacrifice long-term luck to immediately recover probability and temporarily regain control of the fight.

For traversal, `HorizontalMovement` uses acceleration-based side movement instead of snapping instantly to max speed, giving the character more weight and finer control during combat. It also performs collision-aware movement checks to prevent pushing into level geometry. `Jump` supports configurable jump count, variable jump height through button hold, grounded detection, and fall-speed handling, making airborne movement feel much more deliberate than a simple one-force jump. `Dash` is implemented as a separate state that temporarily disables normal movement, applies a burst of horizontal force, and can phase through selected colliders during the dash window, giving the player a strong evasive option during the boss fight.

Combat is split into melee and ranged systems. `MeleeAttackManager` handles chained spear attacks with grounded combo strings, air attacks, downward attacks, and a held finisher, while also applying different forward movement bursts depending on the attack stage. The actual hit detection is handled in `MeleeWeapon`, which evaluates collision with damageable targets, computes damage through the project’s probability-based combat formula, restores player health on successful hits, and applies different recoil responses depending on attack direction. This is one of the main places where the game’s theme becomes mechanical: attacking is not just about dealing damage, but also about changing your current combat odds and recovering momentum.

Ranged combat is managed by `WeaponAttackManager`, which uses an object pool to reuse projectiles instead of instantiating and destroying them at runtime. Firing a shot costs the player part of their current probability resource, reinforcing the idea that every offensive choice carries a direct statistical tradeoff. Projectile spawning is timed through animation events rather than firing immediately on button press, so gameplay stays synchronized with the character’s animation and feedback. The same animation-event pipeline is also used to trigger melee hitboxes at the correct frame.

The player’s survivability system is implemented in `PlayerHealth`, which extends a shared `Health` base class but adds combat-specific behavior such as hit stun, knockback, temporary invulnerability, sprite transparency during i-frames, death handling, and automatic recovery toward the player’s current luck value over time. This recovery behavior is central to the game’s design: the player’s current combat probability is not fixed, but shifts during battle and gradually stabilizes back toward underlying luck. On top of that, the Burn Luck system creates a deliberate comeback mechanic by letting the player lower long-term luck in exchange for short-term survival and stronger offensive pressure. Together, these systems make the player controller more than a movement-and-attack script; they make it the primary vehicle for expressing the game’s unfair-probability combat design in playable form.

**中文**

*Hunting For Luck* 中的玩家控制器采用模块化玩法架构，以 `Character` 类作为共享状态中心，统一管理移动、跳跃、冲刺、近战、远程攻击、输入和生命系统。与其将所有逻辑堆叠在一个大型脚本中，我将主要行为拆分到独立组件中，并通过 `isJumping`、`isGrounded`、`isMeleeAttacking`、`isShooting`、`isDashing`、`isGettingHit`、`isDead` 等共享运行时状态进行协调。这种结构使玩家逻辑更容易扩展，也能让不同机制之间以可预测的方式相互打断或互斥。

输入由 Unity Input System 中的 `InputManager` 统一处理。移动、跳跃、近战攻击、远程攻击、冲刺以及特殊能力 **Burn Luck** 都通过输入回调分发到对应模块中。这样输入层本身保持轻量，而具体行为逻辑则由各个玩法模块分别负责。Burn Luck 尤其关键，因为它将玩家输入直接连接到本作的核心风险收益机制：玩家可以牺牲长期幸运值，立即恢复概率值，并在短时间内重新夺回战斗主动权。

在移动方面，`HorizontalMovement` 采用基于加速度的横向移动，而不是瞬间切换到最大速度，使角色在战斗中拥有更明确的重量感与更细致的操控体验。它还包含基于碰撞检测的移动限制，防止角色挤入场景几何体。`Jump` 支持可配置跳跃次数、通过按键时长控制的可变跳跃高度、落地检测以及下落速度控制，使空中操作相比简单单次施力跳跃更加有层次。`Dash` 则被实现为独立状态：在短时间内禁用常规移动、施加一次横向爆发位移，并可在冲刺窗口内穿过指定碰撞体，为 Boss 战提供强力回避手段。

战斗系统被划分为近战与远程两部分。`MeleeAttackManager` 负责地面连段、空中攻击、下劈攻击以及蓄力终结技，并根据攻击阶段施加不同的前冲位移。实际命中判定由 `MeleeWeapon` 处理，它会检测与可受伤目标的碰撞，按照项目中的概率战斗公式计算伤害，在命中后为玩家回复生命，并根据攻击方向施加不同的后坐反馈。这也是本作主题真正落到机制上的关键部分之一：攻击不仅意味着输出伤害，也意味着改变当前战斗概率并夺回节奏。

远程战斗由 `WeaponAttackManager` 管理，其通过对象池复用投射物，而不是在运行时频繁实例化与销毁。每次发射都会消耗玩家当前的一部分概率资源，进一步强化“每次主动进攻都伴随统计代价”的设计思想。投射物生成并不是在按键瞬间立刻触发，而是通过动画事件精确控制，从而让玩法逻辑与角色动画及反馈保持同步。同样的动画事件管线也被用于在正确帧上开启近战命中判定。

玩家的生存系统由 `PlayerHealth` 实现，它继承自共享的 `Health` 基类，并额外加入了受击硬直、击退、短暂无敌、无敌期间精灵透明化、死亡处理以及向玩家当前幸运值自动回归的恢复逻辑。这种恢复机制是游戏设计的核心之一：玩家当前的战斗概率并不是固定值，而是会在战斗过程中不断波动，并在脱离强烈交互后逐步回归到底层幸运值。除此之外，Burn Luck 系统还提供了明确的逆转手段，让玩家能够牺牲长期资源来换取短期生存与更强的进攻压制。综合来看，这套玩家控制系统不仅仅是一个移动与攻击脚本集合，更是将“以不公平概率为核心的战斗设计”转化为可操作体验的主要载体。

---

## Behaviour Tree Implementation | 行为树实现

**English**

The boss AI in *Hunting For Luck* is driven by a custom Behaviour Tree system implemented entirely from scratch rather than using third-party plugins. The framework is built around a base `Node` class that defines the common tree structure, node state system (`RUNNING`, `SUCCESS`, `FAILURE`), parent-child relationships, and a lightweight shared data context for passing information between nodes.

On top of this foundation, I implemented core composite nodes such as `Selector` and `Sequence`, both of which preserve the current child index across evaluations so the tree can correctly resume long-running actions instead of restarting every frame. I also implemented randomized variants, `RandomSelector` and `RandomPicker`, to support less predictable boss decisions and attack variation, along with a `Successor` decorator node for control-flow wrapping.

The tree itself is hosted through an abstract `AbstractBT` MonoBehaviour, which initializes the root node and evaluates it every physics tick in `FixedUpdate`. This makes it easy for a boss controller to define its own behavior tree by overriding `SetupTree()`. The result is a reusable and extensible AI framework that allowed me to tune pacing, randomness, and multi-phase combat flow at the node level instead of hard-coding all boss logic into a single monolithic script.

**中文**

*Hunting For Luck* 中的 Boss AI 由一个完全从零实现的自定义 Behaviour Tree 系统驱动，而不是依赖第三方插件。整个框架围绕基础 `Node` 类构建，该类定义了通用树结构、节点状态系统（`RUNNING`、`SUCCESS`、`FAILURE`）、父子节点关系，以及用于在节点之间传递信息的轻量共享数据上下文。

在这一基础之上，我实现了 `Selector` 与 `Sequence` 等核心组合节点。这些节点会在多次评估之间保留当前子节点索引，从而使行为树能够在长时间动作执行过程中正确恢复进度，而不是每帧重新开始。我还实现了带随机性的 `RandomSelector` 与 `RandomPicker`，以支持更加不可预测的 Boss 决策与攻击变化，并加入了 `Successor` 装饰节点用于控制流包装。

行为树本体由抽象 `AbstractBT` MonoBehaviour 承载，它负责初始化根节点，并在 `FixedUpdate` 中每个物理帧执行一次树评估。通过重写 `SetupTree()`，具体 Boss 控制器可以方便地定义自己的行为树结构。最终得到的是一个可复用、可扩展的 AI 框架，使我能够在节点层面调节节奏、随机性与多阶段战斗流程，而不必把全部 Boss 逻辑硬编码进一个庞大的脚本中。

---

## Legna Boss AI Implementation | Legna Boss AI 实现

**English**

Legna’s boss AI is built on top of the custom Behaviour Tree framework together with a dedicated boss state controller, rather than relying on Unity visual scripting or external AI tools. The entry point is `LegnaBT`, which assembles the full decision tree in code and connects all behavior nodes for phase 1, phase 2, phase transition, sleep, and death. The overall structure is straightforward at a high level: Legna begins dormant, runs a phase 1 moveset, transitions through a dedicated phase-change sequence, enters a more aggressive phase 2, and finally remains in a death state once defeated. Organizing the fight this way made the boss logic much easier to reason about as a sequence of combat stages instead of a loose collection of animation triggers.

At runtime, the Behaviour Tree is tightly integrated with `LegnaCharacter`, which acts as the boss’s central state container. `LegnaCharacter` tracks the current AI state (`SLEEPING`, `STUN`, `CHASE`, `JUMPATTACK`, `CROSSATTACK`, `SPINATTACK`, `GUARD_COUNTER`, `TWINKLEATTACK`, `FIRE`, `EXCALIBUR`, etc.), the current phase, toughness and stagger state, grounded state, and the player’s distance class. It also exposes a large set of trigger flags driven by animation events, including wake-up completion, stun completion, jump-attack timing, combo finish, spin start, dodge start and end, guard and counter timing, phase-change completion, and teleport-attack completion. In practice, the tree decides what move Legna should perform, while `LegnaCharacter` and animation events determine when each move advances through its internal steps.

A key part of the AI is the boss perception model. Every physics tick, `LegnaCharacter` classifies the player as unseen, point-blank, short-range, or long-range, and stores that result as `playerDistanceClass`. The tree then uses `DetectPlayer` nodes to gate different behavior sets based on that distance bucket. This gives Legna context-sensitive decision-making without requiring deeply nested conditionals. When the player is close, the tree can favor guard counter, dodge, or short-range pressure; when the player is farther away, it can prefer chase, jump attack, spin attack, projectile pressure, or Excalibur. This distance-aware structure is one of the main reasons the boss feels reactive rather than locked into a fixed loop.

Phase 1 is built around a smaller, more readable move pool that teaches the player Legna’s core patterns. In this phase, the tree mainly combines chase, cross-attack strings, jump attack, spin attack, dodge repositioning, and a conditional guard counter. These moves are composed through `Sequence`, `Selector`, `RandomSelector`, and `RandomPicker`, allowing Legna to satisfy tactical requirements while remaining somewhat unpredictable. For example, phase 1 can chase into a jump attack, perform a spinning rush followed by recovery, or use close-range cross-attack strings with optional follow-ups depending on spacing and randomness. This keeps the first half of the fight readable without making it fully deterministic.

Phase 2 expands the moveset and shifts the fight toward heavier pressure, more mobility, and stronger space control. In this phase, the tree adds attacks such as `LFire`, `LBackFire`, `LTwinkleAttack`, `LCalibur`, and additional phase-2 slash sequences, while still reusing chase, dodge, jump attack, and spin attack as core building blocks. This means phase 2 is not implemented as an entirely separate AI system, but as a more aggressive recombination of the same node architecture with new phase-exclusive attacks layered on top. That keeps the codebase reusable while making the boss feel like it has genuinely evolved into a more dangerous second form.

Each move node is implemented as a compact self-contained state machine. Rather than executing an entire move in a single frame, nodes such as `LJumpAttack`, `LSpinAttack`, `LCrossAttack`, `LGuardCounter`, `LBackToCenter`, `LBackFire`, `LFire`, and `LCalibur` track internal progress through `moveIndex`, local timers, and animation-driven trigger flags. This allows a node to return `RUNNING` across multiple frames until the action is truly complete. For example, `LJumpAttack` waits for an animation event before applying the aerial impulse, then waits for landing, and finally waits for the finishing animation before reporting success. `LSpinAttack` advances through startup, slow spin movement, fast spin movement, and recovery. `LCalibur` enters a charge state, applies a pull effect to the player under specific orientation and distance conditions, then releases the attack and exits after the finishing animation. This pattern is consistent across the moves and is what allows the tree to orchestrate long boss actions cleanly.

The AI also reacts directly to combat feedback instead of blindly finishing every move. Most action nodes begin by calling `character.HandleHit()`. This method processes incoming hits, handles guarding, reduces toughness, flashes the boss red, spawns hit effects, and triggers stagger when toughness reaches zero. If Legna is interrupted, the current node usually resets its local triggers, returns `FAILURE`, and hands control back to the higher-level tree so the boss can transition into stun or another valid state. This makes Legna interruptible in a controlled way and ensures the boss respects the player’s punish windows. The separate `LStun` node then handles the stagger animation, stun duration, toughness reset, and re-entry into combat.

One of the most important engineering details is how tightly animation and gameplay are synchronized. `LegnaAnimationEvent` exposes a large set of animation callbacks that flip gameplay booleans or directly spawn effects and attacks. These callbacks mark moments such as the end of wake-up, start of dash windows, finish of combos, counter windows, phase-change checkpoints, projectile spawn, Excalibur activation, cross throwing, explosion patterns, and hitbox activation for specific attack steps. Because of this, the boss AI is not simply “playing animations”; the animations actively drive the exact timing of state transitions, hitboxes, movement bursts, and VFX/SFX, which makes the fight feel much tighter and more deliberate than a timer-only implementation.

Overall, Legna’s AI is implemented as a layered system: `LegnaBT` controls the high-level flow, `DetectPlayer` and `PhaseChecker` determine when specific behavior sets become valid, each move node manages its own multi-step execution, `LegnaCharacter` stores combat state and shared runtime data, and animation events provide frame-accurate synchronization for attacks and transitions. This architecture allowed me to build a two-phase boss that is readable, interruptible, reactive to player distance, and varied enough to stay tense and skill-testing throughout the fight.

**中文**

Legna 的 Boss AI 构建在自定义 Behaviour Tree 框架与专用 Boss 状态控制器之上，而不是依赖 Unity 可视化脚本或外部 AI 工具。入口脚本是 `LegnaBT`，它在代码中组装完整的决策树，并连接 phase 1、phase 2、阶段切换、休眠与死亡等所有行为节点。从高层结构来看，Legna 会先处于休眠状态，然后执行 phase 1 招式组，经过专门的阶段切换流程后进入更具压迫感的 phase 2，最后在被击败后停留在死亡状态。用这种方式组织战斗逻辑，使整个 Boss 行为更容易被理解为一系列战斗阶段，而不是一堆松散的动画触发器。

在运行时，Behaviour Tree 与 `LegnaCharacter` 紧密结合，后者作为 Boss 的中心状态容器。`LegnaCharacter` 负责记录当前 AI 状态（如 `SLEEPING`、`STUN`、`CHASE`、`JUMPATTACK`、`CROSSATTACK`、`SPINATTACK`、`GUARD_COUNTER`、`TWINKLEATTACK`、`FIRE`、`EXCALIBUR` 等）、当前阶段、韧性与失衡状态、落地状态，以及玩家距离类别。它还暴露出大量由动画事件驱动的触发标记，包括起身完成、眩晕结束、跳劈时机、连段结束、旋转起手、闪避开始与结束、防御与反击窗口、阶段切换完成以及传送攻击完成等。实际运行中，行为树负责决定 Legna **要做什么动作**，而 `LegnaCharacter` 与动画事件则决定 **这个动作何时推进到下一个内部步骤**。

AI 的关键组成之一是 Boss 的感知模型。每个物理帧中，`LegnaCharacter` 都会将玩家分类为未发现、贴身距离、近距离或远距离，并将结果记录为 `playerDistanceClass`。行为树随后通过 `DetectPlayer` 节点，根据这个距离分类启用不同的行为集合。这样一来，Legna 就能够根据上下文进行决策，而不需要编写大量深层嵌套条件判断。玩家靠近时，树可以更倾向于使用防反、闪避或近距离压制；玩家拉开距离时，则更可能选择追击、跳劈、旋转突进、远程压制或 Excalibur。正是这种基于距离的结构，让 Boss 表现得更像在主动响应玩家，而不是机械地循环固定招式。

Phase 1 围绕一个更小、更易读的招式池构建，用来让玩家学习 Legna 的核心战斗模式。在这个阶段中，行为树主要组合追击、十字斩连段、跳劈、旋转攻击、闪避调整站位以及条件式防反。这些行为通过 `Sequence`、`Selector`、`RandomSelector` 和 `RandomPicker` 进行组合，使 Legna 既能满足战术需求，又能保持一定程度的不可预测性。例如，phase 1 可以先追击再接跳劈，执行一次旋转突进后进入恢复，或者根据距离与随机条件触发近距离十字斩及其后续派生。这样既能保证前半场战斗的可读性，又不会让其变成完全固定的脚本流程。

Phase 2 则进一步扩展招式池，使战斗转向更强的压制力、更高的机动性和更强的空间控制能力。在这一阶段中，行为树会加入 `LFire`、`LBackFire`、`LTwinkleAttack`、`LCalibur` 以及额外的 phase 2 斩击序列，同时依旧复用追击、闪避、跳劈和旋转攻击等核心模块。这意味着 phase 2 并不是一个完全独立的新 AI 系统，而是在同一套节点架构上叠加新的阶段专属技能，并以更激进的方式重新组合已有行为。这样既保证了代码复用性，也让 Boss 在体验上真正呈现出“二阶段进化”的压迫感。

每个招式节点本身都被实现为一个紧凑的、自包含的小型状态机。它们不会在单帧内一次性执行完整动作，而是像 `LJumpAttack`、`LSpinAttack`、`LCrossAttack`、`LGuardCounter`、`LBackToCenter`、`LBackFire`、`LFire`、`LCalibur` 这样的节点，通过 `moveIndex`、局部计时器和动画驱动的触发标记来记录内部执行进度。因此，节点可以在动作尚未结束时连续多帧返回 `RUNNING`。例如，`LJumpAttack` 会先等待动画事件触发，再施加空中冲量；之后等待落地，最后等待收招动画结束后才返回成功。`LSpinAttack` 会依次经历起手、低速旋转、高速旋转和恢复阶段。`LCalibur` 会先进入蓄力状态，在满足朝向与距离条件时对玩家施加吸附效果，随后释放攻击，并在结束动画后退出。正是这种一致的实现模式，使行为树能够干净地编排持续时间较长的 Boss 动作。

AI 还会直接对战斗反馈作出响应，而不是机械地把当前动作完整做完。大多数动作节点一开始都会调用 `character.HandleHit()`。这个方法负责处理受到的攻击、防御逻辑、韧性削减、Boss 受击变红、受击特效生成，以及当韧性归零时触发失衡。如果 Legna 在行动中被打断，当前节点通常会重置本地触发状态，返回 `FAILURE`，并将控制权交还给更高层的行为树，使其能够切换到眩晕或其他合法状态。这样就让 Legna 的动作打断具有清晰且可控的反馈，也保证 Boss 会尊重玩家打出的惩罚窗口。独立的 `LStun` 节点则负责处理失衡动画、眩晕持续时间、韧性重置以及重新回到战斗流程。

其中一个非常重要的工程实现细节，是动画与玩法逻辑之间的高精度同步。`LegnaAnimationEvent` 暴露出大量动画回调，这些回调会切换玩法布尔状态，或直接生成攻击与特效。它们标记了起身结束、冲刺窗口开始、连段结束、防反窗口、阶段切换检查点、投射物生成、Excalibur 激活、十字投射、爆炸模式，以及特定攻击步骤的命中框激活等关键时刻。因此，Boss AI 并不是简单地“播放动画”；恰恰相反，动画本身在驱动状态切换、命中框开启、位移爆发与 VFX / SFX 的准确时序，这也是战斗手感能够比纯计时器驱动实现更紧凑、更有设计感的重要原因。

总体来看，Legna 的 AI 是一个分层实现的系统：`LegnaBT` 负责高层行为流程控制，`DetectPlayer` 与 `PhaseChecker` 决定哪些行为集合在当前条件下可用，各个动作节点负责自己的多阶段执行过程，`LegnaCharacter` 维护战斗状态与共享运行时数据，而动画事件则为攻击与阶段转换提供帧级精度的同步。这样的架构让我能够实现一个可读、可打断、会根据玩家距离动态响应，并且在整场战斗中持续保持紧张感与挑战性的双阶段 Boss。
