import {
  Select,
  SelectContent,
  SelectGroup,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import type { ZoneId } from "@/types/court";

export function SelectZone({
  setZone,
}: {
  setZone: (zone: ZoneId | null) => void;
}) {
  const items = [
    { label: "Overall", value: null },
    { label: "Paint", value: "paint" },
    { label: "Midrange", value: "midrange" },
    { label: "3PT", value: "threePoint" },
    { label: "Free Throw", value: "freeThrow" },
  ];

  return (
    <Select items={items} onValueChange={(value) => setZone(value as ZoneId | null)}>
      <SelectTrigger className="w-[180px]">
        <SelectValue placeholder="Overall" />
      </SelectTrigger>
      <SelectContent>
        <SelectGroup>
          {items.map((item) => (
            <SelectItem key={item.value} value={item.value}>
              {item.label}
            </SelectItem>
          ))}
        </SelectGroup>
      </SelectContent>
    </Select>
  );
}
