import { DATE_CHARS_PATTERN, DateCharsInputDirective } from './date-chars-input.directive';

describe('DateCharsInputDirective', () => {
  let directive: DateCharsInputDirective;

  beforeEach(() => {
    directive = new DateCharsInputDirective();
  });

  it('should create an instance', () => {
    expect(directive)
      .toBeTruthy();
  });

  it('should have a date chars pattern', () => {
    expect(directive['allowedCharsPattern'])
      .withContext('valid allowed chars pattern')
      .toBe(DATE_CHARS_PATTERN);
  });
});
