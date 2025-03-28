// Дожидаемся полной загрузки DOM-дерева, прежде чем выполнять скрипт.
document.addEventListener('DOMContentLoaded', () => {
    // Функция для получения списка избранных уроков из localStorage.
    // localStorage - это веб-хранилище, позволяющее хранить данные в браузере пользователя между сессиями.
    // Данные хранятся в виде пар ключ-значение.
    function getFavorites() {
        // Пытаемся получить данные по ключу 'favorites'.
        // JSON.parse() преобразует строку JSON (которая хранится в localStorage) обратно в объект JavaScript.
        // Если данных по ключу 'favorites' нет, то localStorage.getItem() вернет null.
        // В этом случае используем оператор ИЛИ (||) и пустой массив '[]', чтобы избежать ошибки.
        return JSON.parse(localStorage.getItem('favorites') || '[]');
    }

    // Функция для сохранения списка избранных уроков в localStorage.
    function setFavorites(favorites) {
        // Сохраняем массив favorites в localStorage под ключом 'favorites'.
        // JSON.stringify() преобразует объект JavaScript (массив favorites) в строку JSON.
        localStorage.setItem('favorites', JSON.stringify(favorites));
    }

    // Функция для переключения состояния избранного урока (добавление/удаление).
    function toggleFavorite(lessonId, button) { // lessonId - ID урока, button - кнопка "звездочка"
        const favorites = getFavorites(); // Получаем текущий список избранного.
        // Ищем урок в массиве `allLessons` по `lessonId`.  `allLessons` - это *глобальная* переменная (не определена в этом коде),
        // которая, предположительно, содержит *все* доступные уроки.
        const lesson = allLessons.find(l => l.id === lessonId);
        if (!lesson) return; // Если урок с таким ID не найден в `allLessons`, выходим из функции.

        // Проверяем, есть ли урок в избранном.
        const isFavorite = favorites.some(fav => fav.id === lessonId); // some() возвращает true, если хотя бы один элемент массива удовлетворяет условию.
        if (isFavorite) {
            // Если урок уже в избранном, удаляем его.
            // filter() создает *новый* массив, содержащий только те элементы, для которых функция-колбэк вернула true.
            const updatedFavorites = favorites.filter(fav => fav.id !== lessonId);
            setFavorites(updatedFavorites); // Сохраняем обновленный список.
            button.style.color = 'black'; // Меняем цвет звездочки на черный.
        } else {
            // Если урока нет в избранном, добавляем его.
            favorites.push(lesson); // Добавляем объект урока в массив.
            setFavorites(favorites); // Сохраняем обновленный список.
            button.style.color = 'yellow'; // Меняем цвет звездочки на желтый.
        }

        // Если текущий URL страницы - '/favorites' (без учета регистра), то обновляем отображение избранных уроков.
        // window.location.pathname - это часть URL после имени домена (например, '/favorites', '/catalog', '/about').
        if (window.location.pathname.toLowerCase() === '/favorites') {
            renderFavorites(); // Вызываем функцию для отрисовки избранного.
        }
    }

    // Функция для отображения избранных уроков на странице /Favorites.
    function renderFavorites() {
        const favoritesContent = document.getElementById('favorites-content'); // Получаем элемент, куда будем вставлять контент.
        if (!favoritesContent) return; // Если такого элемента нет (мы не на странице /Favorites), выходим.

        const favorites = getFavorites(); // Получаем список избранных уроков.
        favoritesContent.innerHTML = ''; // Очищаем содержимое контейнера.

        if (favorites.length === 0) {
            // Если избранных уроков нет, показываем сообщение об этом.
            favoritesContent.innerHTML = '<p>У вас пока нет избранных уроков.</p>';
            return;
        }

        // Группируем избранные уроки по уровням (levelId) и группам (groupId).
        // reduce() - это метод массива, который "сворачивает" массив в одно значение.
        const lessonsByLevelAndGroup = favorites.reduce((acc, lesson) => {
            // acc - это аккумулятор, в котором накапливается результат.
            // lesson - текущий обрабатываемый элемент массива (объект урока).
            if (!acc[lesson.levelId]) acc[lesson.levelId] = {}; // Если уровня еще нет в аккумуляторе, создаем пустой объект.
            if (!acc[lesson.levelId][lesson.groupId]) acc[lesson.levelId][lesson.groupId] = []; // Если группы еще нет, создаем пустой массив.
            acc[lesson.levelId][lesson.groupId].push(lesson); // Добавляем урок в соответствующую группу и уровень.
            return acc; // Возвращаем аккумулятор для следующей итерации.
        }, {}); // {} - начальное значение аккумулятора (пустой объект).

        // Проходим по всем уровням (allLevels - глобальная переменная, содержащая все уровни).
        allLevels.forEach(level => {
            if (!lessonsByLevelAndGroup[level.id]) return; // Если в избранном нет уроков этого уровня, пропускаем его.

            // Создаем секцию для уровня.
            const section = document.createElement('section');
            section.className = `topic-section ${level.name.replace(/\s+/g, '-')}`; // Добавляем классы для стилизации.  /\s+/g - регулярное выражение, заменяющее все пробелы на дефисы.
            section.innerHTML = `<h2 class="visually-hidden">${level.name}</h2>`; // Добавляем заголовок (скрытый, для доступности).

            const ul = document.createElement('ul'); // Создаем список (<ul>).
            ul.className = 'topic-list';

            // Проходим по всем группам уроков (allLessonGroups - глобальная переменная).
            allLessonGroups.forEach(group => {
                // Если в избранном есть уроки этой группы и этого уровня.
                if (lessonsByLevelAndGroup[level.id] && lessonsByLevelAndGroup[level.id][group.id]) {
                    const li = document.createElement('li'); // Создаем элемент списка (<li>).
                    li.className = 'topic-item';
                    // Формируем HTML-код для элемента списка, включая карточки уроков.
                    li.innerHTML = `
                        <button class="topic-button">${group.name}</button>
                        <div class="cards-container" data-level-id="${level.id}" data-group-id="${group.id}">
                            ${lessonsByLevelAndGroup[level.id][group.id].map(lesson => `
                                <a href="${lesson.filePath}" class="card" data-lesson-id="${lesson.id}">
                                    <div class="lesson-info">
                                        <span class="lesson-title">${lesson.title}</span>
                                        <span class="lesson-date">Дата выхода: ${new Date(lesson.releaseDate).toISOString().split('T')[0]}</span>
                                    </div>
                                    <button class="favorite-btn">★</button>
                                </a>
                            `).join('')}
                        </div>
                    `; // map() создает новый массив, применяя функцию к каждому элементу.  join('') объединяет элементы массива в строку.
                    ul.appendChild(li); // Добавляем элемент списка в список.
                }
            });

            section.appendChild(ul); // Добавляем список в секцию.
            favoritesContent.appendChild(section); // Добавляем секцию в контейнер избранного.
        });

        // Навешиваем обработчики событий на кнопки "Избранное" внутри `favoritesContent`.
        attachFavoriteHandlers(favoritesContent);
    }

    // Функция для навешивания обработчиков на кнопки "Избранное" и установки начального состояния звездочек.
    // container - элемент DOM, внутри которого искать кнопки (по умолчанию - весь документ).
    window.attachFavoriteHandlers = function (container = document) {
        const favoriteButtons = container.querySelectorAll('.favorite-btn'); // Находим все кнопки с классом .favorite-btn.
        console.log('Найдено кнопок "Избранное":', favoriteButtons.length); // Выводим количество найденных кнопок в консоль (для отладки).
        favoriteButtons.forEach(button => { // Перебираем все найденные кнопки.
            const card = button.closest('.card'); // Находим ближайший родительский элемент с классом .card (карточка урока).
            const lessonId = parseInt(card.dataset.lessonId, 10); // Получаем ID урока из атрибута data-lesson-id.  parseInt() преобразует строку в число.
            const isFavorite = getFavorites().some(fav => fav.id === lessonId); // Проверяем, есть ли урок в избранном.
            button.style.color = isFavorite ? 'yellow' : 'black'; // Устанавливаем цвет звездочки в зависимости от того, в избранном ли урок.

            // Удаляем старые обработчики событий (если они были), чтобы избежать многократного срабатывания.
            button.removeEventListener('click', handleFavoriteClick);
            // Добавляем новый обработчик события "click".
            button.addEventListener('click', handleFavoriteClick);

            // Функция-обработчик клика по кнопке "Избранное".
            function handleFavoriteClick(event) {
                event.preventDefault(); // Отменяем стандартное действие браузера (переход по ссылке, если кнопка внутри <a>).
                event.stopPropagation(); // Предотвращаем "всплытие" события (чтобы событие не обработалось на родительских элементах).
                toggleFavorite(lessonId, button); // Вызываем функцию для добавления/удаления урока из избранного.
            }
        });
    }

    // Обработка кликов на странице каталога (/Catalog).
    // Используем делегирование событий: вешаем один обработчик на родительский элемент, а не на каждую кнопку.
    const catalogContent = document.getElementById('catalog-content'); // Получаем контейнер каталога.
    if (catalogContent) { // Если мы на странице каталога.
        catalogContent.addEventListener('click', (event) => { // Добавляем обработчик события "click" на контейнер.
            const button = event.target.closest('.favorite-btn'); // event.target - элемент, на котором произошло событие.  closest() ищет ближайший родительский элемент, удовлетворяющий селектору.
            if (button) { // Если клик был по кнопке "Избранное".
                event.preventDefault(); // Отменяем стандартное действие.
                event.stopPropagation(); // Предотвращаем всплытие.
                const card = button.closest('.card'); // Находим карточку урока.
                const lessonId = parseInt(card.dataset.lessonId, 10); // Получаем ID урока.
                toggleFavorite(lessonId, button); // Добавляем/удаляем урок из избранного.
            }
        });

        // Инициализируем состояние звездочек на странице /Catalog.
        attachFavoriteHandlers(catalogContent);
    }

    // Если мы на странице избранного (/Favorites), отображаем избранные уроки.
    if (window.location.pathname.toLowerCase() === '/favorites') {
        renderFavorites();
    }
});