import { ChangeDetectionStrategy, Component, ErrorHandler, OnDestroy, OnInit } from '@angular/core';
import { CommonModule, DecimalPipe, KeyValue } from '@angular/common';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { SelectionModel } from '@angular/cdk/collections';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatSelectChange, MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatChipsModule } from '@angular/material/chips';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';

import {
  BehaviorSubject,
  NEVER,
  Observable,
  Subject,
  Subscription,
  asapScheduler,
  catchError,
  debounceTime,
  defer,
  distinctUntilChanged,
  filter,
  first,
  forkJoin,
  iif,
  map,
  mergeMap,
  of,
  skipWhile,
  startWith,
  switchMap,
  tap
} from 'rxjs';

import {
  CommandMessage,
  DataMessage,
  MessageService,
  MessageStatistics,
  MessageStatisticsOptions,
  MessageTrackOptions,
  Messages,
  MessagesOptions
} from './message.service';

import { DateCharsInputDirective } from '../shared/date-chars-input/date-chars-input.directive';
import { TimeCharsInputDirective } from '../shared/time-chars-input/time-chars-input.directive';
import { StopClickPropagationDirective } from '../shared/stop-click-propagation/stop-click-propagation.directive';

import {
  ConfirmationDialogComponent,
  confirmationDialogConfig,
  confirmationDialogContentPartials,
  getConfirmationDialogContent
} from '../shared/confirmation-dialog/confirmation-dialog.component';

import { MapComponent } from '../shared/map/map.component';
import { TablePaginationComponent } from '../shared/table-pagination/table-pagination.component';

import { PAGE_NUM as INITIAL_PAGE, PaginationOptions } from '../directory-tech/shared/pagination';
import { DEBOUNCE_DUE_TIME, MonitoringTech, SEARCH_MIN_LENGTH } from '../tech/tech.component';

import { TableDataSource } from '../directory-tech/shared/table/table.data-source';
import { LocationAndTrackResponse, MonitoringSensor } from '../tech/tech.service';
import { TrackerParameter } from '../directory-tech/tracker.service';

@Component({
  selector: 'bio-messages',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatAutocompleteModule,
    MatDatepickerModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatTableModule,
    MatCheckboxModule,
    MatChipsModule,
    DateCharsInputDirective,
    TimeCharsInputDirective,
    StopClickPropagationDirective,
    MapComponent,
    TablePaginationComponent
  ],
  providers: [DecimalPipe],
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export default class MessagesComponent implements OnInit, OnDestroy {
  /**
   * Get today's date or lesser end's date.
   *
   * @returns Max start date.
   */
  protected get maxStartDate() {
    let maxStartDate = new Date();

    const endDate = this.selectionForm.get('range.end.date')
      ?.value;

    if (endDate) {
      const isEndDayMax = new Date(endDate)
        .getTime() < maxStartDate.getTime();

      if (isEndDayMax) {
        maxStartDate = new Date(endDate);
      }
    }

    return maxStartDate;
  }

  /**
   * Calculate tomorrow's max date.
   *
   * @returns Max end date.
   */
  protected get maxEndDate() {
    const date = new Date();
    const tomorrowDay = date.getDate() + 1;

    date.setDate(tomorrowDay);

    return date;
  }

  /**
   * Whether the number of selected messages matches the total number of messages.
   *
   * @returns All messages selected value.
   */
  protected get isAllSelected() {
    return this.selection.selected.length === this.messagesDataSource?.data.length;
  }

  /**
   * Get tech search stream.
   *
   * @returns An `Observable` of search stream.
   */
  get #search$() {
    return this.selectionForm
      .get('tech')!
      .valueChanges.pipe(
        filter(value => typeof value !== 'object'),
        map(value => value as string | undefined),
        debounceTime(DEBOUNCE_DUE_TIME),
        map(searchValue => searchValue
          ?.trim()
          ?.toLocaleLowerCase()
        ),
        distinctUntilChanged(),
        skipWhile(searchValue => searchValue ? searchValue.length < SEARCH_MIN_LENGTH : true),
        map(searchValue => searchValue !== undefined && searchValue.length < SEARCH_MIN_LENGTH ? undefined : searchValue),
        distinctUntilChanged()
      );
  }

  /**
   * Get messages search stream.
   *
   * @returns An `Observable` of search stream.
   */
  get #filter$() {
    return this.searchForm
      .get('search')!
      .valueChanges.pipe(
        debounceTime(DEBOUNCE_DUE_TIME),
        map(searchValue => searchValue
          ?.trim()
          ?.toLocaleLowerCase()
        ),
        map(searchValue => searchValue || undefined),
        distinctUntilChanged(),
        startWith(
          this.searchForm.get('search')
            ?.value ?? undefined
        )
      );
  }

  /**
   * Get message options.
   *
   * @returns `MessagesOptions` value.
   */
  get #options() {
    return Object.freeze(this.#messages$.value!);
  }

  /**
   * Set message options, update pagination.
   */
  set #options(options: MessagesOptions) {
    this.#messages$.next({ ...this.#options, ...options });
  }

  protected selectionForm!: MessageSelectionForm;
  protected tech$!: Observable<MonitoringTech[]>;
  protected searchForm!: MessageSearchForm;
  protected messages$?: Observable<Messages>;
  protected messagesDataSource?: TableDataSource<TrackerMessageDataSource | SensorMessageDataSource | CommandMessageDataSource>;
  protected location$?: Observable<LocationAndTrackResponse>;
  protected statistics$?: Observable<MessageStatistics>;
  protected MessageType = MessageType;
  protected DataMessageParameter = DataMessageParameter;
  protected columns?: KeyValue<MessageColumn | string, string | undefined>[];
  protected columnKeys?: string[];
  protected selection = new SelectionModel<TrackerMessageDataSource | SensorMessageDataSource>(true);
  protected MessageColumn = MessageColumn;

  /**
   * Map a tech option's control value to its name display value in the trigger.
   *
   * @param tech `MonitoringTech` tech.
   *
   * @returns `MonitoringTech` display value.
   */
  protected displayFn(tech: MonitoringTech): string {
    return tech?.name ?? '';
  }

  /**
   * `TrackByFunction` to compute the identity of tech.
   *
   * @param index The index of the item within the iterable.
   * @param tech The `MonitoringTech` in the iterable.
   *
   * @returns `MonitoringTech` ID.
   */
  protected techTrackBy(index: number, { id }: MonitoringTech) {
    return id;
  }

  /**
   * Toggle `message` `parameter` control disabled state
   * on `message` `type` selection change conditionally.
   *
   * @param event `MatSelectionChange` event.
   */
  protected onMessageTypeSelectionChange({ value }: MatSelectChange) {
    const parameterControl = this.selectionForm.get('message.parameter');

    value === MessageType.DataMessage ? parameterControl?.enable() : parameterControl?.disable();
  }

  /**
   * Reset selection form default values.
   */
  protected onResetSelectionForm() {
    asapScheduler.schedule(() => {
      this.selectionForm
        .get('range.start.time')
        ?.setValue('00:00');

      this.selectionForm
        .get('range.end.time')
        ?.setValue('00:00');
    }, 1);
  }

  /**
   * Submit selection form, checking validation state.
   *
   * Get message statistics.
   */
  protected async submitSelectionForm() {
    this.#subscription?.unsubscribe();

    const { invalid, value } = this.selectionForm;

    if (invalid) {
      return;
    }

    const { tech, range, message } = value;

    const { start, end } = range!;
    const { type, parameter } = message!;

    const startDate = new Date(start!.date!);
    const endDate = new Date(end!.date!);

    const [startHours, startMinutes] = parseTime(start!.time!);

    startDate.setHours(startHours);
    startDate.setMinutes(startMinutes);

    const [endHours, endMinutes] = parseTime(end!.time!);

    endDate.setHours(endHours);
    endDate.setMinutes(endMinutes);

    const messagesOptions: MessagesOptions = {
      vehicleId: (tech as MonitoringTech).id,
      periodStart: startDate.toISOString(),
      periodEnd: endDate.toISOString(),
      viewMessageType: type!,
      parameterType: parameter
    };

    if (parameter) {
      messagesOptions.parameterType = parameter;
    }

    const messageStatisticsOptions: MessageStatisticsOptions = { ...messagesOptions };

    messagesOptions.pageNum = INITIAL_PAGE;

    const messageTrackOptions: MessageTrackOptions = {
      vehicleId: (tech as MonitoringTech).id,
      periodStart: startDate.toISOString(),
      periodEnd: endDate.toISOString()
    };

    this.#subscription = forkJoin([
      this.messageService.getTrack(messageTrackOptions),
      this.messageService.getStatistics(messageStatisticsOptions),
      this.#messagesSettled$.pipe(
        first()
      )
    ])
      .subscribe(([location, statistics]) => {
        this.#location$.next(location);
        this.#statistics$.next(statistics);
      });

    if (this.#options && (this.#options.viewMessageType !== type || this.#options.parameterType !== parameter)) {
      // reset form when message type changes
      this.searchForm.reset(undefined, {
        emitEvent: false
      });
    }

    this.#options = messagesOptions;
  }

  /**
   * `TrackByFunction` to compute the identity of parameter.
   *
   * @param index The index of the item within the iterable.
   * @param tech The `TrackerParameter` in the iterable.
   *
   * @returns `TrackerParameter` name.
   */
  protected parameterTrackBy(index: number, { paramName }: TrackerParameter) {
    return paramName;
  }

  /**
   * Selects all messages if they are not all selected, otherwise clears selection.
   */
  protected toggleAllRows() {
    if (this.isAllSelected) {
      this.selection.clear();
    } else {
      const { data } = this.messagesDataSource as TableDataSource<TrackerMessageDataSource | SensorMessageDataSource>;

      this.selection.select(...data);
    }
  }

  /**
   * Delete messages in table.
   */
  protected onDeleteMessages() {
    const entity = this.selection.selected.length.toString();

    let { contentStart } = confirmationDialogContentPartials;

    contentStart += 'сообщения (';
    const contentEnd = ')?';

    const data: InnerHTML['innerHTML'] = getConfirmationDialogContent(entity, undefined, contentStart, contentEnd);

    const dialogRef = this.dialog.open<ConfirmationDialogComponent, InnerHTML['innerHTML'], boolean | undefined>(
      ConfirmationDialogComponent,
      { ...confirmationDialogConfig, data }
    );

    const messageIDs = this.selection.selected.map(({ id }) => id);

    const deleteMessages$ = this.#options.viewMessageType === MessageType.DataMessage
      ? this.messageService.deleteMessages(messageIDs)
      : this.messageService.deleteCommandMessages(messageIDs);

    this.#subscription = dialogRef
      .afterClosed()
      .pipe(
        filter(Boolean),
        mergeMap(() => deleteMessages$)
      )
      .subscribe({
        next: () => {
          this.snackBar.open(MESSAGES_DELETED);

          this.#updateMessages();
        },
        error: error => {
          this.errorHandler.handleError(error);

          this.#updateMessages();
        }
      });
  }

  /**
   * Handle table pagination change event.
   *
   * @param pagination `PaginationOptions` options.
   */
  protected onPaginationChange(pagination: PaginationOptions) {
    this.#options = { ...this.#options, ...pagination };
  }

  #messages$ = new BehaviorSubject<MessagesOptions | undefined>(undefined);
  #messagesDataSource?: (TrackerMessageDataSource | CommandMessageDataSource)[];
  #messagesSettled$ = new Subject();
  #location$ = new Subject<LocationAndTrackResponse>();
  #statistics$ = new Subject<MessageStatistics>();
  #subscription: Subscription | undefined;

  /**
   * Validator that requires the control have a tech `object` value from selection.
   *
   * @param control A tech `AbstractControl`.
   *
   * @returns An error map with the `selectionRequired` property
   * if the validation check fails, otherwise `null`.
   */
  #selectionRequiredValidator({ value }: AbstractControl): ValidationErrors | null {
    if (value && typeof value !== 'object') {
      return {
        selectionRequired: true
      };
    }

    return null;
  }

  /**
   * A validator function that requires the range group have a minimum/maximum
   * value of time.
   *
   * @param group A range group.
   *
   * @returns A function that receives a range group and synchronously
   * sets `rangeTimeMin`, `rangeTimeMax` errors, returns a map of `range`
   * validation errors if present, otherwise `null`.
   */
  #rangeTimeValidator(group: AbstractControl): ValidationErrors | null {
    const startDateControl = group.get('start.date');
    const endDateControl = group.get('end.date');
    const startTimeControl = group.get('start.time');
    const endTimeControl = group.get('end.time');

    const isSameDay = startDateControl?.value && endDateControl?.value
      && startDateControl.value.valueOf() === endDateControl.value.valueOf();

    const hasPatternError = startTimeControl?.hasError('pattern') || endTimeControl?.hasError('pattern');

    if (isSameDay && !hasPatternError && startTimeControl?.value && endTimeControl?.value) {
      let startDate = new Date();
      let endDate = new Date();

      if (startDateControl?.value && endDateControl?.value) {
        startDate = new Date(startDateControl.value);
        endDate = new Date(endDateControl.value);
      }

      const [startHours, startMinutes] = parseTime(startTimeControl.value);

      startDate.setHours(startHours);
      startDate.setMinutes(startMinutes);

      const [endHours, endMinutes] = parseTime(endTimeControl.value);

      endDate.setHours(endHours);
      endDate.setMinutes(endMinutes);

      const start = startDate.getTime();
      const end = endDate.getTime();

      if (start >= end) {
        const min = startTimeControl.value;
        const max = endTimeControl.value;

        const startTimeErrors: ValidationErrors = {
          rangeTimeMax: {
            max,
            actual: min
          }
        };

        const endTimeErrors: ValidationErrors = {
          rangeTimeMin: {
            min,
            actual: max
          }
        };

        startTimeControl.setErrors(startTimeErrors);
        endTimeControl.setErrors(endTimeErrors);

        return {
          range: { min, max }
        };
      }
    }

    if (startTimeControl?.hasError('rangeTimeMax')) {
      startTimeControl.setErrors(null);
    }

    if (endTimeControl?.hasError('rangeTimeMin')) {
      endTimeControl.setErrors(null);
    }

    return null;
  }

  /**
   * Initialize Selection form.
   */
  #initSelectionForm() {
    this.selectionForm = this.fb.group({
      tech: this.fb.nonNullable.control<MonitoringTech | string | undefined>(undefined, [
        Validators.required,
        this.#selectionRequiredValidator
      ]),
      range: this.fb.group({
        start: this.fb.group({
          date: this.fb.nonNullable.control<string | undefined>(undefined, Validators.required),
          time: this.fb.nonNullable.control('00:00', [
            Validators.required,
            Validators.pattern(TIME_PATTERN)
          ])
        }),
        end: this.fb.group({
          date: this.fb.nonNullable.control<string | undefined>(undefined, Validators.required),
          time: this.fb.nonNullable.control('00:00', [
            Validators.required,
            Validators.pattern(TIME_PATTERN)
          ])
        })
      }, {
        validators: this.#rangeTimeValidator
      }),
      message: this.fb.group({
        type: this.fb.nonNullable.control<MessageType | undefined>(undefined, Validators.required),
        parameter: this.fb.nonNullable.control<DataMessageParameter | undefined>({
          value: undefined,
          disabled: true
        }, Validators.required)
      })
    });
  }

  /**
   * Initialize Search form.
   */
  #initSearchForm() {
    this.searchForm = this.fb.group({
      search: this.fb.nonNullable.control<string | undefined>(undefined)
    });
  }

  /**
   * Set tech.
   */
  #setTech() {
    const tech$ = defer(
      () => this.messageService.getVehicles()
    );

    const searchTech = (findCriterion: string) => this.messageService.getVehicles({ findCriterion });

    this.tech$ = this.#search$.pipe(
      startWith(undefined),
      switchMap(searchQuery => searchQuery ? searchTech(searchQuery) : tech$)
    );
  }

  /**
   * Set columns, column keys.
   *
   * @param messages Messages.
   */
  #setColumns({ sensorDataMessages }: Messages) {
    switch (this.#options?.viewMessageType) {
      case MessageType.DataMessage:

        switch (this.#options.parameterType) {
          case DataMessageParameter.TrackerData:
            this.columns = trackerMessageColumns;

            break;

          case DataMessageParameter.SensorData: {
            const sensorColumns: KeyValue<string, MonitoringSensor['name']>[] = [];

            sensorDataMessages?.[0]?.sensors?.forEach(({ name }, index) => {
              let key = 'sensor';

              if (index) {
                // start numbering from the 2nd sensor
                key += `-${index + 1}`;
              }

              const column: KeyValue<string, MonitoringSensor['name']> = {
                key,
                value: name
              };

              sensorColumns.push(column);
            });

            this.columns = [...dataMessageColumns, ...sensorColumns];
          }
        }

        break;

      case MessageType.CommandMessage:
        this.columns = commandMessageColumns;
    }

    this.columnKeys = this.columns?.map(({ key }) => key);
  }

  /**
   * Compute message row black box CSS class.
   *
   * @param timeDate Message time date.
   * @param registrationDate Message server registration date.
   *
   * @returns Message CSS class.
   */
  #getBlackBoxClass(timeDate: DataMessage['trackerDateTime'], registrationDate: DataMessage['serverDateTime']) {
    let cssClass: string | undefined;

    if (timeDate) {
      const time = new Date(timeDate)
        .getTime();

      const registration = new Date(registrationDate)
        .getTime();

      const period = registration - time;
      const MINUTE = 60 * 1000;

      switch (true) {
        case period <= 2 * MINUTE:

          break;

        case period <= 10 * MINUTE:
          cssClass = 'black-box';

          break;

        case period <= 30 * MINUTE:
          cssClass = 'black-box-medium';

          break;

        default:
          cssClass = 'black-box-long';
      }
    }

    return cssClass;
  }

  /**
   * Map tracker messages data source.
   *
   * @param messages Messages with pagination.
   *
   * @returns Mapped `TrackerMessageDataSource`.
   */
  #mapTrackerMessagesDataSource({ trackerDataMessages }: Messages) {
    return Object
      .freeze(trackerDataMessages!)
      .map(({
        id,
        num: position,
        serverDateTime: registration,
        trackerDateTime: time,
        speed,
        latitude,
        longitude,
        altitude,
        satNumber: satellites,
        parameters
      }): TrackerMessageDataSource => ({
        id,
        position,
        time,
        registration,
        speed,
        location: { latitude, longitude, satellites },
        altitude,
        parameters,
        class: this.#getBlackBoxClass(time, registration)
      }));
  }

  /**
   * Map sensor messages data source, sensors.
   *
   * @param messages Messages with pagination.
   *
   * @returns Mapped `SensorMessageDataSource`.
   */
  #mapSensorMessagesDataSource({ sensorDataMessages }: Messages) {
    return Object
      .freeze(sensorDataMessages!)
      .map(({
        id,
        num: position,
        serverDateTime: registration,
        trackerDateTime: time,
        speed,
        latitude,
        longitude,
        altitude,
        satNumber: satellites,
        sensors
      }): SensorMessageDataSource => {
        const sensorMap: {
          [key: string]: string;
        } = {};

        sensors?.forEach(({ value, unit }, index) => {
          if (value) {
            let key = 'sensor';

            if (index) {
              // start numbering from the 2nd sensor
              key += `-${index + 1}`;
            }

            if (!isNaN(parseFloat(value))) {
              value = this.decimalPipe.transform(value, '1.1-6', 'en-US')!;
            }

            sensorMap[key] = `${value} ${unit}`;
          }
        });

        return {
          id,
          position,
          time,
          registration,
          speed,
          location: { latitude, longitude, satellites },
          altitude,
          ...sensorMap,
          class: this.#getBlackBoxClass(time, registration)
        };
      });
  }

  /**
   * Map command messages data source.
   *
   * @param messages Messages with pagination.
   *
   * @returns Mapped `CommandMessageDataSource`.
   */
  #mapCommandMessagesDataSource({ commandMessages }: Messages) {
    return Object
      .freeze(commandMessages!)
      .map(({
        id,
        num: position,
        commandDateTime: time,
        commandText: command,
        executionTime: execution,
        channel,
        commandResponseText: response
      }): CommandMessageDataSource => ({ id, position, time, channel, command, execution, response }));
  }

  /**
   * Initialize `TableDataSource` and set messages data source.
   *
   * @param messages Messages.
   */
  #setMessagesDataSource(messages: Messages) {
    let messagesDataSource: (TrackerMessageDataSource | SensorMessageDataSource | CommandMessageDataSource)[];

    switch (this.#options?.viewMessageType) {
      case MessageType.DataMessage:

        switch (this.#options.parameterType) {
          case DataMessageParameter.TrackerData:
            messagesDataSource = this.#mapTrackerMessagesDataSource(messages);

            this.#messagesDataSource = messagesDataSource as TrackerMessageDataSource[];

            break;

          case DataMessageParameter.SensorData:
            messagesDataSource = this.#mapSensorMessagesDataSource(messages);

            this.#messagesDataSource = undefined;
        }

        break;

      case MessageType.CommandMessage:
        messagesDataSource = this.#mapCommandMessagesDataSource(messages);

        this.#messagesDataSource = messagesDataSource as CommandMessageDataSource[];
    }

    if (this.messagesDataSource) {
      this.messagesDataSource.setDataSource(messagesDataSource!);
    } else {
      this.messagesDataSource = new TableDataSource(messagesDataSource!);
    }
  }

  /**
   * Filter `TrackerMessageDataSource` by parameters.
   *
   * @param searchQuery Search query.
   *
   * @returns Filtered `TrackerMessageDataSource[]`.
   */
  #filterTrackerMessagesDataSource(searchQuery: string) {
    const lookupParameter = (
      { query, comparison, value }: NonNullable<RegExpMatchArray['groups']>,
      { paramName, lastValueString, lastValueDecimal, lastValueDateTime }: TrackerParameter
    ) => {
      const queryPattern = new RegExp(`^${query}$`, 'i');

      let hasMatch = queryPattern.test(paramName);

      if (hasMatch && comparison && value !== undefined) {
        const valuePattern = new RegExp(`^${value}$`, 'i');

        switch (true) {
          case (lastValueString !== undefined):
            hasMatch &&= ['=', '<>'].includes(comparison);

            if (hasMatch) {
              const hasValue = valuePattern.test(lastValueString!);

              hasMatch &&= comparison === '=' ? hasValue : !hasValue;
            }

            break;

          case (lastValueDecimal !== undefined):
            switch (comparison) {
              case '<':
                hasMatch &&= lastValueDecimal! < Number(value);

                break;

              case '<=':
                hasMatch &&= lastValueDecimal! <= Number(value);

                break;

              case '=':
                hasMatch &&= lastValueDecimal! === Number(value);

                break;

              case '<>':
                hasMatch &&= lastValueDecimal! !== Number(value);

                break;

              case '>=':
                hasMatch &&= lastValueDecimal! >= Number(value);

                break;

              case '>':
                hasMatch &&= lastValueDecimal! > Number(value);
            }

            break;

          case (lastValueDateTime !== undefined):
            hasMatch &&= ['=', '<>'].includes(comparison);

            if (hasMatch) {
              const hasValue = valuePattern.test(lastValueDateTime!);

              hasMatch &&= comparison === '=' ? hasValue : !hasValue;
            }
        }
      } else {
        const value = lastValueString ?? lastValueDecimal?.toString() ?? lastValueDateTime;

        hasMatch ||= queryPattern.test(value!);
      }

      return hasMatch;
    };

    const queryGroups = searchQuery
      .replaceAll(QUERY_FORBIDDEN_CHARACTERS_PATTERN, '')
      .replaceAll(/\s/g, '')
      .replaceAll('?', '.+')
      .replaceAll('*', '.*')
      .split(',')
      .filter(query => query !== '')
      .map(query => query
        .match(PARAMETER_QUERY_PATTERN)!
        .groups!
      );

    const firstMatchedParameterIndices: number[] = [];

    return (this.#messagesDataSource as TrackerMessageDataSource[])
      .filter(({ parameters }) => parameters?.length && queryGroups.some(groups => parameters.some((parameter, index) => {
        const hasParameter = lookupParameter(groups, parameter);

        if (hasParameter) {
          firstMatchedParameterIndices.push(index);
        }

        return hasParameter;
      })))
      .map((message, index) => {
        message = structuredClone(message);

        queryGroups.forEach(group => {
          for (let i = 0; i < message.parameters!.length; i++) {
            const parameter = message.parameters![i];

            if (parameter.hasHighlight) {
              continue;
            }

            // set highlight for matched parameter index, do lookup for others
            const hasHighlight = firstMatchedParameterIndices[index] === i || lookupParameter(group, parameter);

            if (hasHighlight) {
              parameter.hasHighlight = hasHighlight;

              // colors applied based on queries
              parameter.backgroundColor = parameterColors[i % parameterColors.length];
            }
          }
        });

        return message;
      });
  }

  /**
   * Filter `CommandMessageDataSource` by response.
   *
   * @param searchQuery Search query.
   *
   * @returns Filtered `CommandMessageDataSource[]`.
   */
  #filterCommandMessagesDataSource(searchQuery: string) {
    return (this.#messagesDataSource as CommandMessageDataSource[])
      .filter(({ response }) => response !== undefined && searchQuery
        .replaceAll(QUERY_FORBIDDEN_CHARACTERS_PATTERN, '')
        .replaceAll('?', '.+')
        .replaceAll('*', '.*')
        .split(',')
        .map(query => query.trim())
        .filter(query => query !== '')
        .some(query => {
          const queryPattern = new RegExp(query, 'i');

          const isValid = queryPattern.test(response);

          return isValid;
        })
      );
  }

  /**
   * Filter `TableDataSource` and set messages data source.
   *
   * @param searchQuery Search query.
   */
  #filterMessagesDataSource(searchQuery?: string) {
    let messagesDataSource: (TrackerMessageDataSource | CommandMessageDataSource)[];

    if (searchQuery === undefined) {
      messagesDataSource = this.#messagesDataSource!;
    } else {
      switch (this.#options?.viewMessageType) {
        case MessageType.DataMessage:
          messagesDataSource = this.#filterTrackerMessagesDataSource(searchQuery);

          break;

        case MessageType.CommandMessage:
          messagesDataSource = this.#filterCommandMessagesDataSource(searchQuery);
      }
    }

    this.messagesDataSource!.setDataSource(messagesDataSource);
  }

  /**
   * Set messages, message columns, clear selection.
   */
  #setMessages() {
    const filter$ = defer(() => this.#filter$.pipe(
      tap(searchQuery => {
        this.#filterMessagesDataSource(searchQuery);
      })
    ));

    this.messages$ = this.#messages$.pipe(
      filter((value): value is MessagesOptions => value !== undefined),
      switchMap(messagesOptions => this.messageService
        .getMessages(messagesOptions)
        .pipe(
          tap(messages => {
            this.#setColumns(messages);
            this.#setMessagesDataSource(messages);

            this.selection.clear();

            this.#messagesSettled$.next(undefined);
          }),
          switchMap(messages => iif(() => messagesOptions.parameterType === DataMessageParameter.SensorData, of(undefined), filter$)
            .pipe(
              map(() => messages)
            )
          ),
          catchError(error => {
            this.errorHandler.handleError(error);

            return NEVER;
          })
        )
      )
    );
  }

  /**
   * Emit messages update.
   */
  #updateMessages() {
    this.#messages$.next(this.#options);
  }

  constructor(
    private errorHandler: ErrorHandler,
    private decimalPipe: DecimalPipe,
    private fb: FormBuilder,
    private dialog: MatDialog,
    private snackBar: MatSnackBar,
    private messageService: MessageService
  ) {
    this.location$ = this.#location$.asObservable();
    this.statistics$ = this.#statistics$.asObservable();
  }

  // eslint-disable-next-line @typescript-eslint/member-ordering
  ngOnInit() {
    this.#initSelectionForm();
    this.#setTech();
    this.#initSearchForm();
    this.#setMessages();
  }

  // eslint-disable-next-line @typescript-eslint/member-ordering
  ngOnDestroy() {
    this.#subscription?.unsubscribe();
  }
}

export enum MessageType {
  DataMessage = 'dataMessage',
  CommandMessage = 'commandMessage'
}

export enum DataMessageParameter {
  TrackerData = 'trackerData',
  SensorData = 'sensorData'
}

export enum MessageColumn {
  Selection = 'selection',
  Position = 'position',
  Time = 'time',
  Registration = 'registration',
  Speed = 'speed',
  Location = 'location',
  Altitude = 'altitude',
  Parameters = 'parameters',
  Command = 'command',
  Channel = 'channel',
  Execution = 'execution',
  Response = 'response'
}

type MessageSelectionForm = FormGroup<{
  tech: FormControl<MonitoringTech | string | undefined>;
  range: FormGroup<{
    start: FormGroup<{
      date: FormControl<string | undefined>;
      time: FormControl<string>;
    }>;
    end: FormGroup<{
      date: FormControl<string | undefined>;
      time: FormControl<string>;
    }>;
  }>;
  message: FormGroup<{
    type: FormControl<MessageType | undefined>;
    parameter: FormControl<DataMessageParameter | undefined>;
  }>;
}>;

type MessageSearchForm = FormGroup<{
  search: FormControl<string | undefined>;
}>;

interface TrackerMessageDataSource extends Pick<DataMessage, 'id' | 'speed' | 'altitude'> {
  position: DataMessage['num'];
  time?: DataMessage['trackerDateTime'];
  registration: DataMessage['serverDateTime'];
  location?: {
    latitude: DataMessage['latitude'];
    longitude: DataMessage['longitude'];
    satellites: DataMessage['satNumber'];
  };
  parameters?: (TrackerParameter & {
    hasHighlight?: boolean;
    backgroundColor?: string;
  })[];
  class?: string;
}

interface SensorMessageDataSource extends Pick<DataMessage, 'id' | 'speed' | 'altitude'>, Partial<{
  [key: string]: unknown;
}> {
  position: DataMessage['num'];
  time?: DataMessage['trackerDateTime'];
  registration: DataMessage['serverDateTime'];
  location?: {
    latitude: DataMessage['latitude'];
    longitude: DataMessage['longitude'];
    satellites: DataMessage['satNumber'];
  };
  class?: string;
}

interface CommandMessageDataSource extends Pick<CommandMessage, 'id' | 'channel'> {
  position: CommandMessage['num'];
  time: CommandMessage['commandDateTime'];
  command: CommandMessage['commandText'];
  execution: CommandMessage['executionTime'];
  response: CommandMessage['commandResponseText'];
}

export const TIME_PATTERN = /^(0?[0-9]|1\d|2[0-3]):(0[0-9]|[1-5]\d)$/;

const selectionColumn: KeyValue<MessageColumn, undefined>[] = [
  {
    key: MessageColumn.Selection,
    value: undefined
  }
];

const positionColumn: KeyValue<MessageColumn, string>[] = [
  {
    key: MessageColumn.Position,
    value: '#'
  }
];

export const dataMessageColumns: KeyValue<MessageColumn, string>[] = [
  ...positionColumn,
  {
    key: MessageColumn.Time,
    value: 'Время устройства'
  },
  {
    key: MessageColumn.Registration,
    value: 'Время системы'
  },
  {
    key: MessageColumn.Speed,
    value: 'Скорость, км/ч'
  },
  {
    key: MessageColumn.Location,
    value: 'Координаты (Спутники)'
  },
  {
    key: MessageColumn.Altitude,
    value: 'Высота, м'
  }
];

export const trackerMessageColumns: KeyValue<MessageColumn, string | undefined>[] = [
  ...selectionColumn,
  ...dataMessageColumns,
  {
    key: MessageColumn.Parameters,
    value: 'Параметры'
  }
];

export const commandMessageColumns: KeyValue<MessageColumn, string | undefined>[] = [
  ...selectionColumn,
  ...positionColumn,
  {
    key: MessageColumn.Time,
    value: 'Время'
  },
  {
    key: MessageColumn.Command,
    value: 'Текст сообщения'
  },
  {
    key: MessageColumn.Execution,
    value: 'Время выполнения'
  },
  {
    key: MessageColumn.Channel,
    value: 'Канал'
  },
  {
    key: MessageColumn.Response,
    value: 'Ответ на команду'
  }
];

export const MESSAGES_DELETED = 'Сообщения удалены';

const QUERY_FORBIDDEN_CHARACTERS_PATTERN = /[^\w\d\s-.,:<>=*?]/g;
const PARAMETER_QUERY_PATTERN = /(?<query>[^<=>]+)(?<comparison>[<(<=)=(<>)(>=)>]*)?(?<value>.*)?/;

export const parameterColors = ['250, 213, 101', '187, 249, 101', '101, 200, 249', '249, 101, 212', '249, 101, 113'];

/**
 * Parsing time from user input.
 *
 * @param value Time user input (00:00, 23:59 etc.).
 *
 * @returns Array of hours and minutes.
 */
export const parseTime = (value: string) => value
  .split(':')
  .map(Number);
