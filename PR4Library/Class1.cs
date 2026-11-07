namespace PR4Library
{
    public enum NotificationType
    {
        Info,
        Warning,
        Error,
        System
    }

    public enum SortNotificationBy
    {
        Date,
        Priority,
        Title
    }

    public class Notification
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Content { get; set; }
        public NotificationType Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Priority { get; set; }
        public bool IsRead { get; set; }
    }

    public class NotificationFilterOptions
    {
        public bool? IsRead { get; set; }
        public NotificationType[]? Types { get; set; }
        public string? SearchText { get; set; }
        public int? MinPriority { get; set; }
        public SortNotificationBy? SortBy { get; set; }
        public bool Descending { get; set; }
    }

    public class NotificationService
    {
        public IEnumerable<Notification> FilterAndSort(
            IEnumerable<Notification> notifications,
            NotificationFilterOptions options)
        {
            if (notifications == null)
            {
                return Enumerable.Empty<Notification>();
            }

            if (options == null)
            {
                return notifications;
            }

            var query = notifications.AsQueryable();

            if (options.IsRead.HasValue)
            {
                query = query.Where(n => n.IsRead == options.IsRead.Value);
            }

            if (options.Types != null && options.Types.Length > 0)
            {
                query = query.Where(n => options.Types.Contains(n.Type));
            }

            if (!string.IsNullOrWhiteSpace(options.SearchText))
            {
                var text = options.SearchText.Trim();
                query = query.Where(n =>
                    (n.Title != null && n.Title.Contains(text, StringComparison.OrdinalIgnoreCase)) ||
                    (n.Content != null && n.Content.Contains(text, StringComparison.OrdinalIgnoreCase)));
            }

            if (options.MinPriority.HasValue)
            {
                query = query.Where(n => n.Priority >= options.MinPriority.Value);
            }

            if (options.SortBy.HasValue)
            {
                query = options.SortBy.Value switch
                {
                    SortNotificationBy.Date => options.Descending
                        ? query.OrderByDescending(n => n.CreatedAt)
                        : query.OrderBy(n => n.CreatedAt),

                    SortNotificationBy.Priority => options.Descending
                        ? query.OrderByDescending(n => n.Priority)
                        : query.OrderBy(n => n.Priority),

                    SortNotificationBy.Title => options.Descending
                        ? query.OrderByDescending(n => n.Title)
                        : query.OrderBy(n => n.Title),

                    _ => query
                };
            }

            return query.ToList();
        }
    }
}
