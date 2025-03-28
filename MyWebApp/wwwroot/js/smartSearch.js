document.addEventListener('DOMContentLoaded', () => {
    // Ждем полной загрузки DOM.

    // Получаем элемент input для фильтрации по группам (названиям тем).
    const topicFilter = document.getElementById('topic-filter');

    // Создаем элемент <datalist> для автодополнения (выпадающий список с подсказками).
    const dataList = document.createElement('datalist');
    dataList.id = 'group-suggestions'; // Задаем ID, чтобы связать с input.

    // Связываем <datalist> с <input> через атрибут list.
    topicFilter.setAttribute('list', 'group-suggestions');

    // Добавляем <datalist> в DOM (в родительский элемент topicFilter).
    topicFilter.parentNode.appendChild(dataList);

    // Получаем контейнер для отображения уроков.
    const catalogContent = document.getElementById('catalog-content');

    // Функция для обновления списка предложений (групп) при вводе текста в поле фильтра.
    function updateSuggestions(query) {
        // `query` - текст, введенный пользователем.

        // Формируем URL для AJAX-запроса к серверу.
        const url = `/catalog/search-groups?query=${encodeURIComponent(query)}`;

        // Отправляем AJAX-запрос (GET) для получения списка групп, соответствующих запросу.
        fetch(url, { method: 'GET', headers: { 'Accept': 'application/json' } }) // Ожидаем JSON, но сначала получим как текст для отладки
            .then(response => {
                if (!response.ok) { // Проверка на ошибки (статус не 2xx)
                    throw new Error(`Ошибка сервера: ${response.status}`);
                }
                return response.text(); // Сначала получаем ответ как текст
            })
            .then(text => {
                if (!text) throw new Error('Пустой ответ от сервера'); // Проверка на пустой ответ
                console.log('Ответ сервера:', text); // Логируем текст ответа
                return JSON.parse(text); // Преобразуем текст в JSON-объект.
            })
            .then(data => {
                // `data` - объект с данными о группах (предположительно, { groups: [...] }).

                dataList.innerHTML = ''; // Очищаем предыдущие предложения в <datalist>.

                // Добавляем новые предложения в <datalist>.
                data.groups.forEach(group => {
                    const option = document.createElement('option');
                    option.value = group.name; // Значение (value) каждой опции - название группы.
                    dataList.appendChild(option);
                });
            })
            .catch(error => console.error('Ошибка поиска:', error)); // Обработка ошибок.
    }

    // Функция для фильтрации уроков по названию группы.
    function filterLessonsByGroup(groupName) {
        // `groupName` - название группы, по которой фильтруем.

        // Формируем URL для AJAX-запроса.  Если `groupName` пустое, показываем все уроки.
        const url = groupName
            ? `/catalog/filter-by-group?groupName=${encodeURIComponent(groupName)}`
            : '/catalog/filter'; // Если groupName не указано, используем обычный фильтр

        // Отправляем AJAX-запрос (GET) для получения отфильтрованных уроков.
        fetch(url, { method: 'GET', headers: { 'Accept': 'application/json' } })
            .then(response => response.json()) // Преобразуем ответ в JSON.
            .then(data => {
                // `data` - объект с отфильтрованными уроками (по структуре, аналогичной другим примерам).

                catalogContent.innerHTML = ''; // Очищаем содержимое контейнера.

                // Если уроков по заданной группе нет, показываем сообщение.
                if (Object.keys(data.lessonsByLevelAndGroup).length === 0) {
                    catalogContent.innerHTML = '<p>Уроки по заданной группе не найдены.</p>';
                    return; // Выходим из функции.
                }

                // Генерируем HTML для отображения уроков (аналогично другим примерам).
                data.levels.forEach(level => {
                    if (!data.lessonsByLevelAndGroup[level.id]) return;

                    const section = document.createElement('section');
                    section.className = `topic-section ${level.name.replace(/\s+/g, '-')}`;
                    section.innerHTML = `<h2 class="visually-hidden">${level.name}</h2>`;

                    const ul = document.createElement('ul');
                    ul.className = 'topic-list';

                    data.lessonGroups.forEach(group => {
                        if (data.lessonsByLevelAndGroup[level.id] && data.lessonsByLevelAndGroup[level.id][group.id]) {
                            const li = document.createElement('li');
                            li.className = 'topic-item';
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
                        `;
                            ul.appendChild(li);
                        }
                    });

                    section.appendChild(ul);
                    catalogContent.appendChild(section);
                });
            })
            .catch(error => { // Обработка ошибок.
                console.error('Ошибка при фильтрации уроков:', error);
                catalogContent.innerHTML = '<p>Произошла ошибка при загрузке уроков. Попробуйте позже.</p>';
            });
    }

    // Добавляем обработчик события "input" на поле ввода `topicFilter`.
    // Событие "input" срабатывает *при каждом изменении* содержимого поля (при вводе или удалении символов).
    topicFilter.addEventListener('input', (e) => {
        const query = e.target.value.trim(); // Получаем введенный текст и удаляем пробелы по краям.
        if (query.length > 0) { // Если введено больше 0 символов.
            updateSuggestions(query); // Вызываем функцию для обновления предложений.
        } else {
            dataList.innerHTML = ''; // Если поле пустое, очищаем предложения.
            filterLessonsByGroup(''); //  и показываем все уроки
        }
    });

    // Добавляем обработчик события "change" на поле ввода `topicFilter`.
    // Событие "change" срабатывает, когда пользователь *закончил* ввод и выбрал значение (или потерял фокус).
    topicFilter.addEventListener('change', (e) => {
        const selectedGroup = e.target.value.trim(); // Получаем выбранное значение.
        if (selectedGroup) { // Если группа выбрана.
            filterLessonsByGroup(selectedGroup); // Вызываем функцию для фильтрации уроков по выбранной группе.
        }
    });
});