document.addEventListener('DOMContentLoaded', () => {
    // Получаем элементы фильтров и контейнер для уроков из DOM.
    const levelFilter = document.getElementById('level-filter'); // Выпадающий список для выбора уровня.
    const dateFilter = document.getElementById('date-filter');   // Выпадающий список для выбора даты.
    const catalogContent = document.getElementById('catalog-content'); // Контейнер, куда будем вставлять HTML с уроками.

    // Функция для обновления списка уроков.  Эта функция будет вызываться при изменении фильтров и при загрузке страницы.
    function updateLessons() {
        const levelName = levelFilter.value;  // Получаем выбранное значение уровня (например, "Начальный", "Средний").
        const dateFilterValue = dateFilter.value; // Получаем выбранное значение фильтра по дате (например, "newest", "oldest").

        // Формируем URL для отправки AJAX-запроса на сервер.
        // `/catalog/filter` - это endpoint (URL) на сервере, который обрабатывает запросы фильтрации.
        // `encodeURIComponent()` - кодирует значения параметров URL, чтобы они были безопасными (например, пробелы заменяются на %20).
        const url = `/catalog/filter?levelName=${encodeURIComponent(levelName)}&dateFilter=${encodeURIComponent(dateFilterValue)}`;

        // Отправляем AJAX-запрос на сервер.  AJAX позволяет обновить часть страницы без перезагрузки всей страницы.
        fetch(url, { // fetch() - это современный способ отправки сетевых запросов в JavaScript.
            method: 'GET', // Используем метод GET, так как мы получаем данные с сервера.
            headers: {
                'Accept': 'application/json' // Указываем, что ожидаем получить ответ в формате JSON.
            }
        })
            .then(response => response.json()) // Преобразуем ответ из формата JSON в объект JavaScript.
            .then(data => { // `data` - это объект JavaScript, полученный с сервера.  Он содержит отфильтрованные уроки.
                // Очищаем текущий контент контейнера `catalogContent`.
                catalogContent.innerHTML = '';

                // Если уроков по заданным фильтрам нет, показываем сообщение.
                // Object.keys(data.lessonsByLevelAndGroup).length - проверяем, есть ли ключи (уровни) в объекте `lessonsByLevelAndGroup`.
                if (Object.keys(data.lessonsByLevelAndGroup).length === 0) {
                    catalogContent.innerHTML = '<p>Уроки по заданным фильтрам не найдены.</p>';
                    return; // Выходим из функции, если уроков нет.
                }

                // Генерируем HTML для отображения уроков и вставляем его в `catalogContent`.
                // Проходим по всем уровням, полученным с сервера (`data.levels`).
                data.levels.forEach(level => {
                    if (!data.lessonsByLevelAndGroup[level.id]) return; // Если для этого уровня нет уроков, пропускаем его.

                    // Создаем элемент <section> для каждого уровня.
                    const section = document.createElement('section');
                    section.className = `topic-section ${level.name.replace(/\s+/g, '-')}`; // Добавляем классы для стилизации.
                    section.innerHTML = `<h2 class="visually-hidden">${level.name}</h2>`; // Добавляем заголовок (скрытый).

                    const ul = document.createElement('ul'); // Создаем список (<ul>).
                    ul.className = 'topic-list'; // Добавляем класс.

                    // Проходим по всем группам уроков, полученным с сервера (`data.lessonGroups`).
                    data.lessonGroups.forEach(group => {
                        // Если для этого уровня и этой группы есть уроки.
                        if (data.lessonsByLevelAndGroup[level.id] && data.lessonsByLevelAndGroup[level.id][group.id]) {
                            const li = document.createElement('li'); // Создаем элемент списка (<li>).
                            li.className = 'topic-item'; // Добавляем класс.

                            // Формируем HTML-код для элемента списка, включая карточки уроков.
                            // **Да, вот здесь мы генерируем HTML на стороне клиента (в браузере) на основе данных, полученных с сервера.**
                            li.innerHTML = `
                            <button class="topic-button">${group.name}</button>
                            <div class="cards-container" data-level-id="${level.id}" data-group-id="${group.id}">
                                ${data.lessonsByLevelAndGroup[level.id][group.id].map(lesson => `
                                    <a href="${lesson.filePath}" class="card" data-lesson-id="${lesson.id}">
                                        <div class="lesson-info">
                                            <span class="lesson-title">${lesson.title}</span>
                                            <span class="lesson-date">Дата выхода: ${new Date(lesson.releaseDate).toISOString().split('T')[0]}</span>
                                        </div>
                                        <button class="favorite-btn">★</button>
                                    </a>
                                `).join('')}
                                <button class="load-more-btn" data-offset="2">Загрузить еще</button>
                            </div>
                        `; // map() создает новый массив, применяя функцию к каждому элементу (уроку).  join('') объединяет элементы массива в строку.
                            ul.appendChild(li); // Добавляем элемент списка в список.
                        }
                    });

                    section.appendChild(ul); // Добавляем список в секцию.
                    catalogContent.appendChild(section); // Добавляем секцию в контейнер каталога.
                });
            })
            .catch(error => { // Обрабатываем возможные ошибки при выполнении запроса.
                console.error('Ошибка при фильтрации уроков:', error); // Выводим ошибку в консоль.
                catalogContent.innerHTML = '<p>Произошла ошибка при загрузке уроков. Попробуйте позже.</p>'; // Показываем сообщение об ошибке пользователю.
            });
    }

    // Добавляем обработчики событий на элементы фильтров.
    // Когда значение фильтра изменяется, вызывается функция `updateLessons`.
    levelFilter.addEventListener('change', updateLessons);
    dateFilter.addEventListener('change', updateLessons);

    // Вызываем функцию `updateLessons` при первой загрузке страницы, чтобы отобразить уроки по умолчанию.
    updateLessons();
});