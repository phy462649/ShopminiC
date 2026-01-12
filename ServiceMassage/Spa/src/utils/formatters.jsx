// Date formatters
export function formatDate(date, locale = 'vi-VN') {
  if (!date) return '-';
  return new Date(date).toLocaleDateString(locale);
}

export function formatDateTime(date, locale = 'vi-VN') {
  if (!date) return '-';
  return new Date(date).toLocaleString(locale);
}

export function formatTime(date, locale = 'vi-VN') {
  if (!date) return '-';
  return new Date(date).toLocaleTimeString(locale, {
    hour: '2-digit',
    minute: '2-digit',
  });
}

// Currency formatter
export function formatCurrency(amount, currency = 'VND', locale = 'vi-VN') {
  if (amount === null || amount === undefined) return '-';
  return new Intl.NumberFormat(locale, {
    style: 'currency',
    currency,
    maximumFractionDigits: 0,
  }).format(amount);
}

// Number formatter
export function formatNumber(number, locale = 'vi-VN') {
  if (number === null || number === undefined) return '-';
  return new Intl.NumberFormat(locale).format(number);
}

// Phone formatter
export function formatPhone(phone) {
  if (!phone) return '-';
  // Format: 0xxx xxx xxx
  const cleaned = phone.replace(/\D/g, '');
  if (cleaned.length === 10) {
    return `${cleaned.slice(0, 4)} ${cleaned.slice(4, 7)} ${cleaned.slice(7)}`;
  }
  return phone;
}

// Truncate text
export function truncate(text, maxLength = 50) {
  if (!text) return '';
  if (text.length <= maxLength) return text;
  return text.slice(0, maxLength) + '...';
}
