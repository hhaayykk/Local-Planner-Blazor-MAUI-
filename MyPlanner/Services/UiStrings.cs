namespace MyPlanner.Services;

internal static class UiStrings
{
    private static readonly Dictionary<string, Dictionary<UiLanguage, string>> Data = Build();

    public static string Get(string key, UiLanguage lang)
    {
        if (!Data.TryGetValue(key, out var row))
            return key;
        if (row.TryGetValue(lang, out var s))
            return s;
        return row[UiLanguage.En];
    }

    private static Dictionary<string, Dictionary<UiLanguage, string>> Build()
    {
        var d = new Dictionary<string, Dictionary<UiLanguage, string>>();

        void A(string key, string en, string ru, string hy) =>
            d[key] = new Dictionary<UiLanguage, string>
            {
                [UiLanguage.En] = en,
                [UiLanguage.Ru] = ru,
                [UiLanguage.Hy] = hy
            };

        A("App_ShortTitle", "Local Planner", "Локальный планировщик", "Լոկալ պլանավորիչ");
        A("Nav_History", "History", "История", "Պատմություն");
        A("Nav_SiteAria", "Links", "Разделы", "Բաժիններ");
        A("NotFound_Message", "This page does not exist.", "Запрашиваемая страница не существует.", "Տվյալ էջը գոյություն չունի:");

        A("Theme_AriaToDark", "Switch to dark theme", "Переключить на тёмную тему", "Անցնել մուգ թեմայի");
        A("Theme_AriaToLight", "Switch to light theme", "Переключить на светлую тему", "Անցնել լուսավոր թեմայի");

        A("Lang_PickLanguage", "Language", "Язык интерфейса", "Ինտերֆեյսի լեզուն");
        A("Lang_NameEn", "English", "English", "English");
        A("Lang_NameRu", "Russian", "Русский", "Русский");
        A("Lang_NameHy", "Armenian", "Հայերեն", "Հայերեն");

        A("Home_PageTitle", "Local Planner", "Планировщик задач", "Պլանավորիչ");
        A("Home_Activities", "Activities", "Активности", "Ակտիվություններ");
        A("Home_TotalFmt", "{0} total", "Всего: {0}", "Ընդամենը՝ {0}");
        A("Home_SearchAria", "Search activities", "Поиск по активностям", "Որոնել ակտիվությունները");
        A("Home_SearchPlaceholder", "Search title, notes, tags…", "Поиск по заголовку, заметкам, тегам…", "Որոնում ըստ վերնագրի, նշումների, թեգերի…");
        A("Home_FilterAria", "Filters", "Фильтры", "Զտիչներ");
        A("Home_GlanceAria", "Today at a glance", "Кратко на сегодня", "Այսօրվա ամփոփագիր");
        A("Home_OverdueFmt", "{0} overdue", "Просрочено: {0}", "Ժամկետանց՝ {0}");
        A("Home_DoneFmt", "{0} done", "Завершено: {0}", "Ավարտված՝ {0}");

        A("Overlay_CloseFilters", "Close filters", "Закрыть фильтры", "Փակել զտիչները");
        A("Overlay_FiltersTitle", "Filters", "Фильтрация", "Զտում");
        A("Overlay_Close", "Close", "Закрыть", "Փակել");
        A("Overlay_DueFrom", "Due from", "Срок с", "Ժամկետի սկիզբ");
        A("Overlay_DueTo", "Due to", "Срок по", "Ժամկետի հանձնում");
        A("Overlay_Status", "Status", "Статус", "Կարգավիճակ");
        A("Overlay_Any", "Any", "Любой", "Ցանկացած");
        A("Overlay_ClearFilters", "Clear filters", "Сбросить фильтры", "Մաքրել զտիչները");

        A("Common_Loading", "Loading…", "Загрузка…", "Բեռնում...");

        A("Form_NewActivity", "New activity", "Новая активность", "Նոր ակտիվություն");
        A("Form_Basics", "Basics", "Основная информация", "Հիմնական տվյալներ");
        A("Form_TitleReq", "Title *", "Заголовок *", "Վերնագիր *");
        A("Form_Tags", "Tags", "Теги", "Թեգեր");
        A("Form_TagsPlaceholder", "Optional, e.g. report, q1", "Необязательно (напр. отчет, q1)", "Ոչ պարտադիր (օր. զեկույց, q1)");
        A("Form_NotesReq", "Notes *", "Заметки *", "Նշումներ *");
        A("Form_Schedule", "Schedule", "Расписание", "Ժամանակացույց");
        A("Form_StartDate", "Start date", "Дата начала", "Մեկնարկի ամսաթիվ");
        A("Form_Deadline", "Deadline", "Крайний срок", "Վերջնաժամկետ");
        A("Form_Status", "Status", "Статус", "Կարգավիճակ");
        A("Form_Saving", "Saving…", "Сохранение…", "Պահպանում...");
        A("Form_SaveActivity", "Save", "Сохранить", "Պահպանել");
        A("Form_ResetForm", "Reset", "Очистить", "Մաքրել");
        A("Form_DefaultNotes", "Describe this activity in at least ten characters.", "Опишите активность (минимум 10 символов).", "Նկարագրեք ակտիվությունը (առնվազն 10 նիշ):");

        A("List_TitleFmt", "Your activities ({0})", "Ваши активности ({0})", "Ձեր ակտիվությունները ({0})");
        A("List_ScopeAria", "Scope", "Область видимости", "Տեսանելիության տիրույթ");
        A("List_All", "All", "Все", "Բոլորը");
        A("List_Active", "Active", "Текущие", "Ընթացիկ");
        A("List_Done", "Done", "Завершенные", "Ավարտված");
        A("List_ClearFilters", "Clear filters", "Сбросить фильтры", "Մաքրել զտիչները");
        A("List_TagsLine", "Tags:", "Теги:", "Թեգեր:");
        A("List_Due", "Due", "Срок", "Ժամկետ");
        A("List_Added", "Added", "Добавлено", "Ավելացված է");
        A("List_Overdue", "Overdue", "Просрочено", "Ժամկետանց");
        A("List_Edit", "Edit", "Изменить", "Խմբագրել");
        A("List_Delete", "Delete", "Удалить", "Ջնջել");
        A("List_EditorTitle", "Title", "Заголовок", "Վերնագիր");
        A("List_EditorNotes", "Notes", "Заметки", "Նշումներ");
        A("List_EditorStart", "Start", "Начало", "Սկիզբ");
        A("List_EditorDeadline", "Deadline", "Срок", "Վերջնաժամկետ");
        A("List_EditorStatus", "Status", "Статус", "Կարգավիճակ");
        A("List_Save", "Save", "Сохранить", "Պահպանել");
        A("List_Cancel", "Cancel", "Отмена", "Չեղարկել");
        A("List_NoMatch", "No rows match your filters. Try clearing filters or changing search.", "Нет данных, соответствующих фильтрам. Попробуйте изменить параметры поиска.", "Զտիչներին համապատասխանող տվյալներ չկան: Փորձեք փոխել որոնման պարամետրերը:");
        A("List_NoActivities", "No activities yet. Use the form on the left to add one.", "Активностей пока нет. Используйте форму слева для добавления.", "Ակտիվություններ դեռ չկան: Օգտագործեք ձախ կողմի ձևը նորը ավելացնելու համար:");

        A("Pg_AriaNav", "Activity list pages", "Навигация по страницам", "Էջերի նավարկում");
        A("Pg_First", "First", "В начало", "Սկիզբ");
        A("Pg_Prev", "Previous", "Назад", "Նախորդ");
        A("Pg_PageFmt", "Page {0} of {1}", "Страница {0} из {1}", "Էջ {0}, {1}-ից");
        A("Pg_Next", "Next", "Вперед", "Հաջորդ");
        A("Pg_Last", "Last", "В конец", "Վերջ");
        A("Pg_AriaFirst", "First page", "Первая страница", "Առաջին էջ");
        A("Pg_AriaPrev", "Previous page", "Предыдущая страница", "Նախորդ էջ");
        A("Pg_AriaNext", "Next page", "Следующая страница", "Հաջորդ էջ");
        A("Pg_AriaLast", "Last page", "Последняя страница", "Վերջին էջ");

        A("Pulse_StatusMix", "Status mix", "Распределение статусов", "Կարգավիճակների բաշխում");
        A("Pulse_StatusMixEmpty", "Add activities to see how work is spread across statuses.", "Добавьте данные, чтобы увидеть статистику по статусам.", "Ավելացրեք ակտիվություններ՝ ըստ կարգավիճակների վիճակագրությունը տեսնելու համար:");
        A("Pulse_StatusBarAria", "Share of activities by status", "Доля активностей по статусам", "Ակտիվությունների մասնաբաժինն ըստ կարգավիճակի");
        A("Pulse_Upcoming", "Upcoming", "Предстоящие", "Առաջիկա");
        A("Pulse_UpcomingEmpty", "No open items with future deadlines.", "Нет активных задач с будущим сроком.", "Ապագա վերջնաժամկետով բաց առաջադրանքներ չկան:");
        A("Pulse_NeedsAttention", "Needs attention", "Требуют внимания", "Ուշադրության կարիք ունեցող");
        A("Pulse_NeedsEmpty", "Nothing overdue right now.", "На данный момент просроченных задач нет.", "Այս պահին ժամկետանց առաջադրանքներ չկան:");
        A("Pulse_OverdueFmt", "{0} overdue tasks", "Просроченных задач: {0}", "Ժամկետանց առաջադրանքներ՝ {0}");
        A("Pulse_PlannerTip", "Planner tip", "Совет по планированию", "Պլանավորման խորհուրդ");
        A("Pulse_OverviewAria", "Planner overview", "Обзор планировщика", "Պլանավորիչի տեսություն");
        A("Pulse_TipFull",
            "Each morning, check Upcoming, then use Your activities on the Active tab so completed items stay out of the way. All data stays on this device only.",
            "Каждое утро проверяйте «Предстоящие», затем работайте во вкладке «Текущие», чтобы выполненные задачи не отвлекали. Все данные хранятся только на этом устройстве.",
            "Ամեն առավոտ ստուգեք «Առաջիկա» բաժինը, ապա աշխատեք «Ընթացիկ» ներդիրում, որպեսզի ավարտված գործերը չխանգարեն: Բոլոր տվյալները պահվում են միայն այս սարքի վրա:");

        A("Status_NotStarted", "Not started", "Не начата", "Չի սկսվել");
        A("Status_InProgress", "In progress", "В процессе", "Ընթացքի մեջ");
        A("Status_Completed", "Completed", "Завершена", "Ավարտված");
        A("Status_OnHold", "On hold", "На паузе", "Կասեցված");
        A("Status_Cancelled", "Cancelled", "Отменена", "Չեղարկված");

        A("val_title", "Title must be between 3 and 100 characters.", "Заголовок должен содержать от 3 до 100 символов.", "Վերնագիրը պետք է լինի 3-ից 100 նիշ:");
        A("val_notes", "Notes must be between 10 and 1000 characters.", "Заметки должны содержать от 10 до 1000 символов.", "Նշումները պետք է լինեն 10-ից 1000 նիշ:");
        A("val_status", "Pick a valid status.", "Выберите корректный статус.", "Ընտրեք վավեր կարգավիճակ:");
        A("val_tags", "Tags must be at most 200 characters.", "Теги не должны превышать 200 символов.", "Թեգերը չպետք է գերազանցեն 200 նիշը:");

        A("err_load", "Could not load activities.", "Не удалось загрузить активности.", "Չհաջողվեց բեռնել ակտիվությունները:");
        A("err_save", "Could not save the activity.", "Не удалось сохранить активность.", "Չհաջողվեց պահպանել ակտիվությունը:");
        A("err_save_io", "Save failed. Check that the app can write to its data folder.", "Ошибка записи. Проверьте права доступа к папке данных.", "Պահպանման սխալ: Ստուգեք հավելվածի հասանելիությունը տվյալների թղթապանակին:");
        A("err_update", "Could not update status.", "Не удалось обновить статус.", "Չհաջողվեց թարմացնել կարգավիճակը:");
        A("err_update_generic", "Update failed.", "Ошибка обновления.", "Թարմացման սխալ:");
        A("err_delete", "Could not delete activity.", "Не удалось удалить активность.", "Չհաջողվեց ջնջել ակտիվությունը:");
        A("err_delete_generic", "Delete failed.", "Ошибка удаления.", "Ջնջման սխալ:");
        A("err_clear", "Could not clear completed items.", "Не удалось очистить завершенные задачи.", "Չհաջողվեց մաքրել ավարտված տարրերը:");
        A("err_edit_save", "Save failed.", "Ошибка сохранения.", "Պահպանման սխալ:");


        A("Hist_Title", "Deleted activities", "Корзина удаленных задач", "Ջնջված ակտիվություններ");
        A("Hist_Lead", "Everything you delete from the planner is listed here until you clear this log (your device only).", "Все удаленные задачи хранятся здесь до очистки журнала (только на этом устройстве).", "Պլանավորիչից ջնջված ամեն ինչ պահվում է այստեղ մինչև մատյանի մաքրումը (միայն այս սարքում):");
        A("Hist_DeletedLabel", "Deleted", "Удалено", "Ջնջված է");
        A("Hist_IdFmt", "ID {0}", "ID {0}", "ID {0}");
        A("Hist_Empty", "Nothing deleted yet. Removed tasks will appear here.", "Список удаленных задач пуст.", "Ջնջված ակտիվությունների ցուցակը դատարկ է:");
        A("Hist_Clear", "Clear history", "Очистить журнал", "Մաքրել պատմությունը");
        A("Hist_ClearConfirm", "Erase the whole deletion log from this device?", "Полностью очистить журнал удалений на этом устройстве?", "Ջնջե՞լ ջնջումների ամբողջ պատմությունը այս սարքից:");
        A("Hist_ClearYes", "Erase log", "Очистить", "Մաքրել");
        A("Hist_Back", "Back to planner", "Вернуться к планировщику", "Հետ դեպի պլանավորիչ");

        A("err_hist_load", "Could not load deletion history.", "Не удалось загрузить журнал удалений.", "Չհաջողվեց բեռնել ջնջման պատմությունը:");
        A("err_hist_clear", "Could not clear history.", "Не удалось очистить журнал.", "Չհաջողվեց մաքրել պատմությունը:");


        A("Rel_JustNow", "just now", "только что", "հենց հիմա");
        A("Rel_MinAgo", "{0}m ago", "{0} мин. назад", "{0}ր առաջ");
        A("Rel_HoursAgo", "{0}h ago", "{0} ч. назад", "{0}ժ առաջ");
        A("Rel_DaysAgo", "{0}d ago", "{0} дн. назад", "{0}օր առաջ");

        return d;
    }
}