import {
  Tooltip,
  TooltipContent,
  TooltipProvider,
  TooltipTrigger,
} from "@/components/ui/tooltip";

export interface SparklinePoint {
  date: string;
  value: number; // e.g. percentage, 0-100
  label?: string; // shown in the hover tooltip, e.g. "8/10 (80%)"
}

interface SparklineProps {
  points: SparklinePoint[];
  width?: number;
  height?: number;
}

// Renders a simple line + dot sparkline. Points are the source of truth for the
// hover tooltip, so callers should pass a human-readable `label` per point rather
// than relying on this component to reformat raw numbers.
export function Sparkline({ points, width = 280, height = 64 }: SparklineProps) {
  if (points.length === 0) {
    return (
      <p className="text-sm text-muted-foreground">Not enough data yet.</p>
    );
  }

  const padding = 8;
  const values = points.map((p) => p.value);
  const min = Math.min(...values);
  const max = Math.max(...values);
  const range = max - min || 1; // avoid divide-by-zero when all values are equal

  const toX = (i: number) =>
    points.length === 1
      ? width / 2
      : padding + (i / (points.length - 1)) * (width - padding * 2);
  const toY = (v: number) =>
    height - padding - ((v - min) / range) * (height - padding * 2);

  const linePath = points
    .map((p, i) => `${i === 0 ? "M" : "L"} ${toX(i)} ${toY(p.value)}`)
    .join(" ");

  return (
    <TooltipProvider delayDuration={100}>
      <svg
        viewBox={`0 0 ${width} ${height}`}
        width="100%"
        height={height}
        role="img"
        aria-label="Trend sparkline"
      >
        <path
          d={linePath}
          fill="none"
          stroke="currentColor"
          strokeWidth={2}
          className="text-primary"
        />
        {points.map((p, i) => (
          <Tooltip key={`${p.date}-${i}`}>
            <TooltipTrigger asChild>
              <circle
                cx={toX(i)}
                cy={toY(p.value)}
                r={4}
                className="fill-primary cursor-pointer"
              />
            </TooltipTrigger>
            <TooltipContent>
              <p className="text-xs font-medium">{p.date}</p>
              <p className="text-xs text-muted-foreground">
                {p.label ?? `${p.value}%`}
              </p>
            </TooltipContent>
          </Tooltip>
        ))}
      </svg>
    </TooltipProvider>
  );
}