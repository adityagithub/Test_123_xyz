import {
  AfterViewInit,
  Component,
  ElementRef,
  NgZone,
  OnDestroy,
  OnInit,
  ViewChild,
} from '@angular/core';

interface Epic {
  label: string;
  start: string; // ISO date
  end: string;   // ISO date
  color: string; // Tailwind bg-* class
  row?: number;  // assigned row (global grid)
}

interface Swimlane {
  name: string;
  items: Epic[];
}

@Component({
  selector: 'app-timeline',
  templateUrl: './timeline.component.html',
  styleUrls: ['./timeline.component.scss'], // optional; safe to remove if unused
})
export class TimelineComponent implements OnInit, AfterViewInit, OnDestroy {

  months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun',
    'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];

  timelineStart = new Date('2023-01-01');
  timelineEnd = new Date('2023-12-31');
  private totalMs = this.timelineEnd.getTime() - this.timelineStart.getTime();

  /** Row sizing (affects both epics and swimlane label height) */
  epicHeight = 60;   // px -> matches Tailwind h-12
  rowGap = 10;    // px -> vertical space between rows
  lanePadding = 10;    // px -> top + bottom padding inside each lane
  epicMargin = 30;     // top padding inside lane

  swimlanes: Swimlane[] = [
    {
      name: 'E-Comm',
      items: [
        { label: 'Core Pages', start: '2023-02-01', end: '2023-05-30', color: 'bg-blue-500' },
        { label: 'Purchase Flow', start: '2023-04-01', end: '2023-08-15', color: 'bg-blue-600' },
        { label: 'Localization', start: '2023-09-01', end: '2023-11-30', color: 'bg-blue-400' },
      ],
    },
    {
      name: 'Mobile',
      items: [
        { label: 'Mobile API', start: '2023-03-01', end: '2023-06-30', color: 'bg-orange-500' },
        { label: 'Push Notifications', start: '2023-07-01', end: '2023-10-31', color: 'bg-orange-600' },
        { label: 'App Refresh', start: '2023-11-01', end: '2023-12-20', color: 'bg-orange-700' },
      ],
    },
    {
      name: 'Infrastructure',
      items: [
        { label: 'Data Backbone', start: '2023-01-25', end: '2023-03-15', color: 'bg-yellow-500' },
        { label: 'Cloud Connect', start: '2023-04-01', end: '2023-06-15', color: 'bg-yellow-600' },
        { label: 'Real-time Stream', start: '2023-07-01', end: '2023-11-30', color: 'bg-yellow-700' },
        { label: 'Disaster Recovery', start: '2023-12-01', end: '2023-12-31', color: 'bg-red-500' },
      ],
    },
    {
      name: 'Infrastructure2',
      items: [
        { label: 'Data Backbone', start: '2023-01-15', end: '2023-03-15', color: 'bg-yellow-500' },
        { label: 'Cloud Connect', start: '2023-04-01', end: '2023-06-15', color: 'bg-yellow-600' },
        { label: 'Real-time Stream', start: '2023-07-01', end: '2023-11-30', color: 'bg-yellow-700' },
        { label: 'Disaster Recovery', start: '2023-12-01', end: '2023-12-31', color: 'bg-red-500' },
      ],
    },
    {
      name: 'Infrastructure3',
      items: [
        { label: 'Data Backbone', start: '2023-01-15', end: '2023-03-15', color: 'bg-yellow-500' },
        { label: 'Cloud Connect', start: '2023-04-01', end: '2023-06-15', color: 'bg-yellow-600' },
        { label: 'Real-time Stream', start: '2023-07-01', end: '2023-11-30', color: 'bg-yellow-700' },
        { label: 'Disaster Recovery', start: '2023-12-01', end: '2023-12-31', color: 'bg-red-500' },
      ],
    },
    {
      name: 'E-Comm',
      items: [
        { label: 'Core Pages', start: '2023-02-01', end: '2023-05-30', color: 'bg-blue-500' },
        { label: 'Purchase Flow', start: '2023-04-01', end: '2023-08-15', color: 'bg-blue-600' },
        { label: 'Localization', start: '2023-09-01', end: '2023-11-30', color: 'bg-blue-400' },
      ],
    },
    {
      name: 'Mobile',
      items: [
        { label: 'Mobile API', start: '2023-03-01', end: '2023-06-30', color: 'bg-orange-500' },
        { label: 'Push Notifications', start: '2023-07-01', end: '2023-10-31', color: 'bg-orange-600' },
        { label: 'App Refresh', start: '2023-11-01', end: '2023-12-20', color: 'bg-orange-700' },
      ],
    },
  ];

  @ViewChild('swimlaneScroll', { static: false })
  swimlaneScroll!: ElementRef<HTMLDivElement>;

  @ViewChild('epicScroll', { static: false })
  epicScroll!: ElementRef<HTMLDivElement>;

  private removeEpicScrollListener?: () => void;

  constructor(private zone: NgZone) { }


  ngOnInit(): void {
    this.assignRowsGlobally();
  }

  ngAfterViewInit(): void {
    this.zone.runOutsideAngular(() => {
      const swimlaneEl = this.swimlaneScroll?.nativeElement;
      const epicEl = this.epicScroll?.nativeElement;
      if (!swimlaneEl || !epicEl) return;

      const onEpicScroll = () => {
        if (swimlaneEl.scrollTop !== epicEl.scrollTop) {
          swimlaneEl.scrollTop = epicEl.scrollTop;
        }
      };

      epicEl.addEventListener('scroll', onEpicScroll, { passive: true });

      this.removeEpicScrollListener = () => {
        epicEl.removeEventListener('scroll', onEpicScroll as EventListener);
      };
    });
  }

  ngOnDestroy(): void {
    if (this.removeEpicScrollListener) this.removeEpicScrollListener();
  }

  /* ---------------- Layout / Computations ---------------- */

  /**
   * Assign non-overlapping rows globally (shared grid across ALL swimlanes),
   * so epic boxes align vertically between lanes and never overlap.
   */
  private assignRowsGlobally(): void {
    const allEpics = this.swimlanes.flatMap(l => l.items);

    // // Sort by start date to lay out left-to-right
    // const sorted = allEpics.sort((a, b) => {
    //   const da = new Date(a.start).getTime();
    //   const db = new Date(b.start).getTime();
    //   if (da !== db) return da - db;
    //   // Stable ordering for equal starts (shorter first)
    //   return new Date(a.end).getTime() - new Date(b.end).getTime();
    // });

    for (const epic of allEpics) {
      this.assignRows(epic);
    }
  }

  /** Assign compact rows PER LANE */
  assignRows(lane: any): void {
    const rows: any[][] = []; // each row is an array of epics

    lane.items.forEach((epic: any) => {
      let placed = false;

      // try to place in existing rows
      for (let r = 0; r < rows.length; r++) {
        const overlap = rows[r].some(
          (e) => !(epic.end < e.start || epic.start > e.end)
        );
        if (!overlap) {
          epic.row = r;
          rows[r].push(epic);
          placed = true;
          break;
        }
      }

      // if not placed, create a new row
      if (!placed) {
        epic.row = rows.length;
        rows.push([epic]);
      }
    });
  }


  /* ---------------- Template Helpers ---------------- */

  getLaneHeight(lane: any): string {
    if (!lane.items?.length) return this.epicHeight + 2 * this.lanePadding + 'px';

    this.assignRows(lane);

    const rowCount = Math.max(...lane.items.map((e: any) => e.row)) + 1;

    const height =
      rowCount * this.epicHeight +
      (rowCount - 1) * this.rowGap +
      2 * this.lanePadding;

    return height + 'px';
  }

  /** Converts a date to percentage from the left edge of the timeline. */
  getLeftPercent(start: string): string {
    const diff = new Date(start).getTime() - this.timelineStart.getTime();
    const pct = (diff / this.totalMs) * 100;
    return (Math.max(0, Math.min(100, pct))).toFixed(2) + '%';
  }

  /** Converts a (start,end) date range to a width percentage of the timeline. */
  getWidthPercent(start: string, end: string): string {
    const ms = new Date(end).getTime() - new Date(start).getTime();
    const pct = (ms / this.totalMs) * 100;
    return (Math.max(0, pct)).toFixed(2) + '%';
  }


  /** How many stacked rows are in this lane? */
  laneRowCount(lane: any): number {
    if (!lane?.items?.length) return 1;            // at least one row of space
    const maxRow = Math.max(...lane.items.map((e: any) => e.row ?? 0));
    return maxRow + 1;                              // rows are 0-indexed
  }

  /** Total lane height in px (prevents overflow & off-by-one) */
  laneHeightPx(lane: any): number {
    const rows = this.laneRowCount(lane);
    // rows * epicHeight + (rows-1)*rowGap + top&bottom padding
    return rows * this.epicHeight + (rows - 1) * this.rowGap + 2 * this.lanePadding;
  }


  /** Top offset for an epic within its lane */
  epicTopPx(epic: any): number {
    const row = epic?.row ?? 0;
    // top padding + row * (height + gap)
    return this.lanePadding + row * (this.epicHeight + this.rowGap);
  }


  getEpicTop(epic: any): string {
    return this.lanePadding + epic.row * (this.epicHeight + this.rowGap) + 'px';
  }
}
