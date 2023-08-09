import { RelativeTimePipe, timeUnits } from './relative-time.pipe';

describe('RelativeTimePipe', () => {
  let pipe: RelativeTimePipe;

  beforeEach(() => {
    pipe = new RelativeTimePipe();
  });

  it('create an instance', () => {
    expect(pipe)
      .toBeTruthy();
  });

  it('should transform date to relative time in years', () => {
    testRelativeTime(pipe, timeUnits.year, '7 лет назад', -7);
    testRelativeTime(pipe, timeUnits.year, '2 года назад', -2);
    testRelativeTime(pipe, timeUnits.year, 'в прошлом году', -1);
    testRelativeTime(pipe, timeUnits.year, 'в следующем году');
    testRelativeTime(pipe, timeUnits.year, 'через 2 года', 2);
    testRelativeTime(pipe, timeUnits.year, 'через 7 лет', 7);
  });

  it('should transform date to relative time in months', () => {
    testRelativeTime(pipe, timeUnits.month, '11 месяцев назад', -11);
    testRelativeTime(pipe, timeUnits.month, '2 месяца назад', -2);
    testRelativeTime(pipe, timeUnits.month, 'в прошлом месяце', -1);
    testRelativeTime(pipe, timeUnits.month, 'в следующем месяце');
    testRelativeTime(pipe, timeUnits.month, 'через 2 месяца', 2);
    testRelativeTime(pipe, timeUnits.month, 'через 11 месяцев', 11);
  });

  it('should transform date to relative time in days', () => {
    testRelativeTime(pipe, timeUnits.day, '30 дней назад', -30);
    testRelativeTime(pipe, timeUnits.day, '5 дней назад', -5);
    testRelativeTime(pipe, timeUnits.day, '3 дня назад', -3);
    testRelativeTime(pipe, timeUnits.day, 'позавчера', -2);
    testRelativeTime(pipe, timeUnits.day, 'вчера', -1);
    testRelativeTime(pipe, timeUnits.day, 'завтра');
    testRelativeTime(pipe, timeUnits.day, 'послезавтра', 2);
    testRelativeTime(pipe, timeUnits.day, 'через 3 дня', 3);
    testRelativeTime(pipe, timeUnits.day, 'через 5 дней', 5);
    testRelativeTime(pipe, timeUnits.day, 'через 30 дней', 30);
  });

  it('should transform date to relative time in hours', () => {
    testRelativeTime(pipe, timeUnits.hour, '23 часа назад', -23);
    testRelativeTime(pipe, timeUnits.hour, '5 часов назад', -5);
    testRelativeTime(pipe, timeUnits.hour, '2 часа назад', -2);
    testRelativeTime(pipe, timeUnits.hour, '1 час назад', -1);
    testRelativeTime(pipe, timeUnits.hour, 'через 1 час');
    testRelativeTime(pipe, timeUnits.hour, 'через 2 часа', 2);
    testRelativeTime(pipe, timeUnits.hour, 'через 5 часов', 5);
    testRelativeTime(pipe, timeUnits.hour, 'через 23 часа', 23);
  });

  it('should transform date to relative time in minutes', () => {
    testRelativeTime(pipe, timeUnits.minute, '59 минут назад', -59);
    testRelativeTime(pipe, timeUnits.minute, '5 минут назад', -5);
    testRelativeTime(pipe, timeUnits.minute, '2 минуты назад', -2);
    testRelativeTime(pipe, timeUnits.minute, '1 минуту назад', -1);
    testRelativeTime(pipe, timeUnits.minute, 'через 1 минуту');
    testRelativeTime(pipe, timeUnits.minute, 'через 2 минуты', 2);
    testRelativeTime(pipe, timeUnits.minute, 'через 5 минут', 5);
    testRelativeTime(pipe, timeUnits.minute, 'через 59 минут', 59);
  });

  it('should transform date to relative time in seconds', () => {
    testRelativeTime(pipe, timeUnits.second, '59 секунд назад', -59);
    testRelativeTime(pipe, timeUnits.second, '5 секунд назад', -5);
    testRelativeTime(pipe, timeUnits.second, '2 секунды назад', -2);
    testRelativeTime(pipe, timeUnits.second, '1 секунду назад', -1);
    testRelativeTime(pipe, timeUnits.second, 'через 1 секунду');
    testRelativeTime(pipe, timeUnits.second, 'через 2 секунды', 2);
    testRelativeTime(pipe, timeUnits.second, 'через 5 секунд', 5);
    testRelativeTime(pipe, timeUnits.second, 'через 59 секунд', 59);
  });

  it('should transform invalid date ', () => {
    let relativeTime = pipe.transform(undefined);

    expect(relativeTime)
      .withContext('transform invalid `undefined` date')
      .toBeNull();

    relativeTime = pipe.transform('');

    expect(relativeTime)
      .withContext('transform invalid `\'\'` date')
      .toBeNull();

    relativeTime = pipe.transform(NaN);

    expect(relativeTime)
      .withContext('transform invalid `NaN` date')
      .toBeNull();
  });
});

/**
 * Test relative time transformation.
 *
 * @param pipe `RelativeTimePipe` instance.
 * @param unitTime Unit time.
 * @param expected The expected value to compare against.
 * @param number Number of times in past or future.
 */
export function testRelativeTime(pipe: RelativeTimePipe, unitTime: number, expected: string, number = 1) {
  const now = Date.now();
  const unitsTime = number * unitTime;
  const time = now + unitsTime;

  const date = new Date(time);

  const relativeTime = pipe.transform(date);

  expect(relativeTime)
    .withContext('transform relative time')
    .toBe(expected);
}
