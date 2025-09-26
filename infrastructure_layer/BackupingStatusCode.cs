// * Файл "BackupingStatusCode.cs": коды состояния бэкапа. *
/*
 * 0 - ошибка копирования;
 * 1 - копирование успешно выполнено;
 * 2 - отсутствуют файлы для копирования.
 */ 

enum BackupingStatusCode
{
    BACKUP_FAILURE = 0,
    BACKUP_SUCCESS = 1,
    BACKUP_NOT_FOUND = 2
}
