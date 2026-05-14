using System.ComponentModel.DataAnnotations;

namespace MyPlanner.Models
{
    public enum ActivityStatus
    {
        NotStarted,
        InProgress,
        Completed,
        OnHold,
        Cancelled
    }

    public class Activity
    {
        [Required(ErrorMessage = "Id Обязатльно!")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Название обязательно")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Название должно быть от 3 до 100 символов")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Дата создания обязательна")]
        [DataType(DataType.Date)]
        public DateTime Created { get; set; }

        [Required(ErrorMessage = "Дедлайн обязателен")]
        [DataType(DataType.Date)]
        public DateTime Deadline { get; set; }

        [Required(ErrorMessage = "Описание обязательно")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "Описание должно быть от 10 до 1000 символов")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Статус обязателен")]
        [EnumDataType(typeof(ActivityStatus), ErrorMessage = "Некорректный статус")]
        public ActivityStatus Status { get; set; }

        [StringLength(200)]
        public string Tags { get; set; } = string.Empty;
    }
}
