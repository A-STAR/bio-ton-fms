import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'relativeTime',
  standalone: true
})
export class RelativeTimePipe implements PipeTransform {
  /**
   * Transform date to relative time.
   *
   * @param date The date expression: a `Date` object, a number
   * (milliseconds since UTC epoch), or an ISO string (https://www.w3.org/TR/NOTE-datetime).
   *
   * @returns A date string in the relative time format.
   */
  transform(date: Date | string | number | null | undefined): string | null {
    let relativeTime: string | null = null;

    if (date != null && date !== '' && date === date) {
      const relativeTimeFormat = new Intl.RelativeTimeFormat(localeID, {
        numeric: 'auto'
      });

      const now = Date.now();

      const time = new Date(date)
        .getTime();

      const elapsedTime = time - now;

      for (const unit in timeUnits) {
        const unitTime = timeUnits[unit as RelativeTimeFormatUnit];

        /* `Math.abs` accounts for both past and future scenarios */
        const timeDiff = Math.abs(elapsedTime);

        if (timeDiff >= unitTime || unit === 'second') {
          const unitElapse = Math.round(elapsedTime / unitTime);

          relativeTime = relativeTimeFormat.format(unitElapse, unit as RelativeTimeFormatUnit);

          break;
        }
      }
    }

    return relativeTime;
  }
}

type RelativeTimeFormatUnit = 'year' | 'month' | 'day' | 'hour' | 'minute' | 'second';

export const localeID = 'ru-RU';

const second = 1000;
const minute = 60 * second;
const hour = 60 * minute;
const day = 24 * hour;
const year = 365 * day;
const month = year / 12;

export const timeUnits: {
  [key in RelativeTimeFormatUnit]: number;
} = { year, month, day, hour, minute, second };
