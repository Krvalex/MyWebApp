document.addEventListener('DOMContentLoaded', () => {
    // Ждем, пока весь HTML-документ будет загружен и разобран.

    // Находим контейнер, в котором находятся все уроки.  Предполагается, что этот элемент всегда есть на странице.
    const catalogContent = document.getElementById('catalog-content');
    console.log('Контейнер catalog-content найден:', !!catalogContent); // Проверка, найден ли элемент (для отладки).  !! преобразует значение в boolean.

    // Функция для подгрузки дополнительных уроков (реализация "бесконечного скролла" или "показать больше").
    function loadMoreLessons(button) { // `button` - это кнопка "Загрузить ещё", по которой кликнул пользователь.
        console.log('Клик по кнопке "Загрузить ещё"');

        // Получаем контейнер, в который будем добавлять новые уроки.  Это родительский элемент кнопки.
        const cardsContainer = button.parentElement;

        // Получаем ID уровня и группы уроков из атрибутов `data-*` контейнера.
        const levelId = cardsContainer.dataset.levelId;
        const groupId = cardsContainer.dataset.groupId;

        // Получаем текущее смещение (offset) из атрибута `data-offset` кнопки.
        // `offset` - это количество уже загруженных уроков.  Он нужен, чтобы сервер знал, какие уроки отправлять дальше.
        let offset = parseInt(button.dataset.offset, 10); // `parseInt()` преобразует строку в целое число.
        console.log('Параметры запроса:', { levelId, groupId, offset }); // Выводим параметры в консоль (для отладки).

        // Формируем URL для AJAX-запроса к серверу.
        const url = `/catalog/load-more?levelId=${levelId}&groupId=${groupId}&offset=${offset}`;
        console.log('URL запроса:', url);

        // Отправляем AJAX-запрос на сервер.
        fetch(url, {
            method: 'GET', // Используем метод GET, так как получаем данные.
            headers: {
                'Accept': 'application/json' // Указываем, что ожидаем получить ответ в формате JSON.
            }
        })
            .then(response => { // `.then()` - обрабатываем ответ от сервера.
                console.log('Статус ответа:', response.status); // Выводим статус ответа (для отладки).
                if (!response.ok) { // Если статус ответа не 200 OK (т.е. произошла ошибка).
                    throw new Error(`Ошибка сервера: ${response.status}`); // Генерируем ошибку.
                }
                return response.json(); // Преобразуем ответ из JSON в объект JavaScript.
            })
            .then(data => { // `data` - это объект JavaScript, полученный с сервера.  Он содержит новые уроки и информацию о том, есть ли еще уроки.
                console.log('Полученные данные:', data); // Выводим данные в консоль (для отладки).

                // Если в ответе есть уроки, добавляем их в контейнер.
                if (data.lessons.length > 0) {
                    data.lessons.forEach(lesson => { // Перебираем массив уроков.
                        // Создаем элемент <a> для каждой карточки урока.
                        const lessonCard = document.createElement('a');
                        lessonCard.href = lesson.filePath; // Устанавливаем ссылку на урок.
                        lessonCard.className = 'card'; // Добавляем класс для стилизации.
                        lessonCard.dataset.lessonId = lesson.id;  // Добавляем data-атрибут с ID урока.
                        // Формируем HTML-код для карточки урока.
                        lessonCard.innerHTML = `
                        <div class="lesson-info">
                            <span class="lesson-title">${lesson.title}</span>
                            <span class="lesson-date">Дата выхода: ${new Date(lesson.releaseDate).toISOString().split('T')[0]}</span>
                        </div>
                        <button class="favorite-btn">★</button>
                    `;
                        // Добавляем карточку урока в контейнер `cardsContainer` *перед* кнопкой "Загрузить ещё".
                        cardsContainer.insertBefore(lessonCard, button);
                    });

                    // Обновляем значение `offset` в атрибуте `data-offset` кнопки.
                    // Увеличиваем `offset` на количество загруженных уроков, чтобы при следующем нажатии сервер знал, с какого места продолжать.
                    button.dataset.offset = offset + data.lessons.length;
                }

                // Если уроков больше нет (сервер вернул `hasMore: false`), заменяем кнопку на сообщение "Больше уроков нет".
                if (!data.hasMore) {
                    // button.outerHTML = '<p>Больше уроков нет</p>'; //более изящно, чем textContent
                    button.replaceWith(document.createTextNode('Больше уроков нет'));
                }
            })
            .catch(error => { // Обрабатываем возможные ошибки (например, ошибка сети или ошибка на сервере).
                console.error('Ошибка при подгрузке уроков:', error); // Выводим ошибку в консоль.
                button.textContent = 'Ошибка загрузки. Попробуйте позже'; // Показываем сообщение об ошибке пользователю.
            });
    }

    // Используем *делегирование событий*.  Вешаем *один* обработчик клика на *родительский* элемент `catalogContent`,
    // а не на каждую кнопку "Загрузить ещё".
    catalogContent.addEventListener('click', (event) => {
        // `event` - объект события.  `event.target` - элемент, на котором произошло событие (на котором кликнули).

        // Проверяем, был ли клик по кнопке с классом `load-more-btn`.
        // `closest()` ищет ближайший родительский элемент (или сам элемент), соответствующий указанному CSS-селектору.
        const button = event.target.closest('.load-more-btn');
        if (button) { // Если клик был по кнопке "Загрузить ещё".
            console.log('Клик по кнопке "Загрузить ещё" обнаружен');
            loadMoreLessons(button); // Вызываем функцию `loadMoreLessons`, передавая ей кнопку.
        }
    });
});