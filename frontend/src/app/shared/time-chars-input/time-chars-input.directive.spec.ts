import { TIME_CHARS_PATTERN, TimeCharsInputDirective } from './time-chars-input.directive';

describe('TimeCharsInputDirective', () => {
  let directive: TimeCharsInputDirective;

  beforeEach(() => {
    directive = new TimeCharsInputDirective();
  });

  it('should create an instance', () => {
    expect(directive)
      .toBeTruthy();
  });

  it('should have a time chars pattern', () => {
    expect(directive['allowedCharsPattern'])
      .withContext('valid allowed chars pattern')
      .toBe(TIME_CHARS_PATTERN);
  });
});
