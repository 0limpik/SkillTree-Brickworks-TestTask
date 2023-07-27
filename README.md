# Skill Tree - [ТЗ тестового задания](/Тестовое%20задание%20Unity.pdf)
[`Запустить WebGL build в браузере`](https://0limpik.github.io/SkillTree-Brickworks-TestTask/) 

<img src="https://github.com/0limpik/SkillTree-Brickworks-TestTask/assets/50516863/c9bf4ed0-779c-49b3-abec-ba7118524781" height="300" />


Возможность сохранения стоит добавлять указав для каждой способности ее уникальный id и в SkillLearnService добавив метод для загрузки уже изученных скилов.
Сохранение и загрузку реализовать в отдельном сервисе сериализующем изученные скилы. Для его внедрения лучше уже будет добавить boot-сцену и DI.

Также нужна возможность добавить unit тесты для проверки класс SkillTree, которые будут содержать невалидные виды деревьев.  

### `UI`
* **SkillTreeUI** - EntryPoint, строит или инициализирует дерево из SkillTreeConfig's
* **SkinLearnUI** - Инициирует логику изучения скилов и пополнения кошелька 
* **SkillInfoUI** - Отображает кошелек пользоваетля и текущею способность.

### `Configuration`

* **SkillConfig** - Способность использующая как id
* **SkillNodeConfig** - Узел в дереве
* **SkillTreeConfig** - Список узлов для добавление в дерево

### `Model`

* **SkillNode** - Узел со ссылками на необходимые и доступные узлы в дереве
* **SkillTree** - Дерево со всеми узлами
* **SkillTreeContainer** - Контейнер содержащий методы построения дерева.
  
### `Services`

* **PlayerLearnService** - Сервис инкасулирующий логику кошелька игрока и изучения способностей
  * **PlayerWalletService** - Сервис кошелека игрока
  * **SkillLearnService** - Сервис для изучения и забывания способностей
* **SkillCostService** - Сервис стоимости способностей
