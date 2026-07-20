import { useState } from "react";
import { Button } from "@/components/ui/button";
import { Field, FieldLabel } from "@/components/ui/field";
import { Input } from "@/components/ui/input";
import {
  Sheet,
  SheetContent,
  SheetHeader,
  SheetTitle,
  SheetDescription,
  SheetFooter,
  SheetClose,
} from "@/components/ui/sheet";
import type { CourtMode, ZoneId, ZoneStats } from "@/types/court";

const BELOW_ARC = "M42,380 L42,301 A168,168 0 0 1 358,301 L358,380 Z";
const PAINT_RECT = "M140,250 L260,250 L260,380 L140,380 Z";
const FT_CIRCLE_BULGE = "M140,250 A60,60 0 0 1 260,250 Z";
const COURT_RECT = "M20,20 L380,20 L380,380 L20,380 Z";

const ZONE_PATHS: Record<ZoneId, string> = {
  paint: PAINT_RECT,
  freeThrow: FT_CIRCLE_BULGE,
  midrange: `${BELOW_ARC} ${PAINT_RECT} ${FT_CIRCLE_BULGE}`,
  threePoint: `${COURT_RECT} ${BELOW_ARC}`,
};

const ZONE_LABEL_POS: Record<ZoneId, { x: number; y: number }> = {
  paint: { x: 200, y: 340 },
  freeThrow: { x: 200, y: 222 },
  midrange: { x: 90, y: 340 },
  threePoint: { x: 200, y: 170 },
};

interface CourtVisualizationProps {
  mode: CourtMode;
  zones: ZoneStats[];
  /**
   * Fired when a zone's makes/attempts change in edit mode.
   * Currently a stub — parent decides what to do with the change.
   * No API/mutation call here.
   */
  onZoneChange?: (zoneId: ZoneId, makes: number, attempts: number) => void;
}

export function CourtVisualization({
  mode,
  zones,
  onZoneChange,
}: CourtVisualizationProps) {
  const [activeZoneId, setActiveZoneId] = useState<ZoneId | null>(null);
  const activeZone = zones.find((z) => z.id === activeZoneId) ?? null;

  const percentage = (z: ZoneStats) =>
    z.attempts > 0 ? Math.round((z.makes / z.attempts) * 100) : 0;

  const handleZoneTap = (zoneId: ZoneId) => {
    if (mode === "view") return; // view mode: zones are display-only, no sheet
    setActiveZoneId(zoneId);
  };

  return (
    <>
      <svg
        viewBox="0 0 400 400"
        className="w-full max-w-sm mx-auto"
        role={mode === "edit" ? undefined : "img"}
        aria-label={mode === "edit" ? undefined : "Court shot zones"}
      >
        {/* Court boundary (sideline / half-court box) */}
        <rect
          x="20"
          y="20"
          width="360"
          height="360"
          fill="none"
          stroke="currentColor"
          strokeWidth="2"
          className="text-muted-foreground"
        />

        {/* Decorative, non-interactive court markings */}
        <g
          fill="none"
          stroke="currentColor"
          strokeWidth="1.5"
          className="text-muted-foreground/60"
          pointerEvents="none"
        >
          {/* Lane / key outline */}
          <path d={PAINT_RECT} />
          {/* Free-throw circle (full circle, dashed on the lane side) */}
          <path d="M140,250 A60,60 0 0 1 260,250" />
          <path d="M140,250 A60,60 0 0 0 260,250" strokeDasharray="4 4" />
          {/* Backboard */}
          <line x1="179" y1="350" x2="221" y2="350" strokeWidth="2.5" />
          {/* Restricted area arc */}
          <path d="M172,358 A28,28 0 0 1 228,358" />
          {/* Three-point line */}
          <path d="M42,380 L42,301 A168,168 0 0 1 358,301 L358,380" />
          {/* Half-court circle (cut off at top edge) */}
          <path d="M155,20 A45,45 0 0 0 245,20" />
        </g>

        {/* Hoop */}
        <circle
          cx="200"
          cy="358"
          r="6"
          fill="none"
          stroke="currentColor"
          strokeWidth="2"
          pointerEvents="none"
        />

        <g role={mode === "edit" ? "group" : undefined}>
          {zones.map((zone) => (
            <g key={zone.id}>
              <path
                d={ZONE_PATHS[zone.id]}
                fillRule="evenodd"
                className={
                  mode === "edit"
                    ? "fill-orange-500/10 stroke-orange-500 stroke-2 hover:fill-orange-500/20 cursor-pointer transition-colors"
                    : "fill-orange-500/10 stroke-orange-500 stroke-2"
                }
                onClick={() => handleZoneTap(zone.id)}
                tabIndex={mode === "edit" ? 0 : -1}
                role={mode === "edit" ? "button" : undefined}
                aria-label={`${zone.label}: ${zone.makes} of ${zone.attempts} made`}
                onKeyDown={(e) => {
                  if (mode === "edit" && (e.key === "Enter" || e.key === " ")) {
                    e.preventDefault();
                    handleZoneTap(zone.id);
                  }
                }}
              />
              <text
                x={ZONE_LABEL_POS[zone.id].x}
                y={ZONE_LABEL_POS[zone.id].y}
                textAnchor="middle"
                className="fill-foreground text-xs font-medium pointer-events-none select-none"
              >
                {zone.makes}/{zone.attempts} · {percentage(zone)}%
              </text>
            </g>
          ))}
        </g>
      </svg>

      {mode === "edit" && (
        <Sheet
          open={activeZoneId !== null}
          onOpenChange={(open) => !open && setActiveZoneId(null)}
        >
          <SheetContent side="bottom">
            {activeZone && (
              <ZoneActionSheetContent
                zone={activeZone}
                onZoneChange={onZoneChange}
                onClose={() => setActiveZoneId(null)}
              />
            )}
          </SheetContent>
        </Sheet>
      )}
    </>
  );
}

interface ZoneActionSheetContentProps {
  zone: ZoneStats;
  onZoneChange?: (zoneId: ZoneId, makes: number, attempts: number) => void;
  onClose: () => void;
}

function ZoneActionSheetContent({
  zone,
  onZoneChange,
  onClose,
}: ZoneActionSheetContentProps) {
  // Local display state only — mirrors the zone prop so the sheet feels
  // responsive to taps. Not persisted anywhere; parent owns real state
  // once save logic is built.
  const [makes, setMakes] = useState(zone.makes);
  const [attempts, setAttempts] = useState(zone.attempts);

  const emit = (nextMakes: number, nextAttempts: number) => {
    setMakes(nextMakes);
    setAttempts(nextAttempts);
    onZoneChange?.(zone.id, nextMakes, nextAttempts);
  };

  const handleMake = () => emit(makes + 1, attempts + 1);
  const handleMiss = () => emit(makes, attempts + 1);

  return (
    <>
      <SheetHeader>
        <SheetTitle>{zone.label}</SheetTitle>
        <SheetDescription>
          Tap Make or Miss to record a shot, or edit the totals directly.
        </SheetDescription>
      </SheetHeader>

      <div className="px-4 space-y-4">
        <div className="flex flex-col gap-2">
          <Button className="w-full" onClick={handleMake}>
            Make
          </Button>
          <Button className="w-full" variant="outline" onClick={handleMiss}>
            Miss
          </Button>
          {/* Undo intentionally omitted — not needed yet per current scope. */}
        </div>

        <div className="flex gap-3">
          <Field className="flex-1">
            <FieldLabel htmlFor={`${zone.id}-makes`}>Makes</FieldLabel>
            <Input
              id={`${zone.id}-makes`}
              type="number"
              min={0}
              value={makes}
              onChange={(e) => {
                const next = Math.max(0, Number(e.target.value));
                emit(next, Math.max(next, attempts));
              }}
            />
          </Field>
          <Field className="flex-1">
            <FieldLabel htmlFor={`${zone.id}-attempts`}>Attempts</FieldLabel>
            <Input
              id={`${zone.id}-attempts`}
              type="number"
              min={0}
              value={attempts}
              onChange={(e) => {
                const next = Math.max(0, Number(e.target.value));
                emit(Math.min(makes, next), next);
              }}
            />
          </Field>
        </div>
      </div>

      <SheetFooter>
        <SheetClose
          render={
            <Button variant="outline" className="w-full" onClick={onClose}>
              Close
            </Button>
          }
        ></SheetClose>
      </SheetFooter>
    </>
  );
}
