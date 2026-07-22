import { Controller, type Control } from "react-hook-form";
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@/components/ui/popover";
import { Calendar } from "@/components/ui/calendar";
import { Button } from "@/components/ui/button";
import type { SessionFormValues } from "../schemas/sessionFormSchema";
import { Field, FieldError, FieldLabel } from "@/components/ui/field";
import { format, parse } from "date-fns";

interface DatePickerFieldProps {
  control: Control<SessionFormValues>;
}

const DATE_FORMAT = "yyyy-MM-dd";

function parseStoredDate(value: string): Date {
  return parse(value, DATE_FORMAT, new Date());
}

export function DatePickerField({ control }: DatePickerFieldProps) {
  return (
    <Controller
      name="date"
      control={control}
      render={({ field, fieldState }) => (
        <Field className="mx-auto w-44" data-invalid={fieldState.invalid}>
          <FieldLabel htmlFor="date-picker-simple">Date</FieldLabel>
          <Popover>
            <PopoverTrigger
              render={
                <Button
                  variant="outline"
                  id="date-picker-simple"
                  className="justify-start font-normal"
                  aria-invalid={fieldState.invalid}
                >
                  {field.value ? (
                    format(parseStoredDate(field.value), "PPP")
                  ) : (
                    <span>Pick a date</span>
                  )}
                </Button>
              }
            />
            <PopoverContent className="w-auto p-0" align="start">
              <Calendar
                mode="single"
                selected={field.value ? parseStoredDate(field.value) : undefined}
                onSelect={(date) =>
                  field.onChange(date ? format(date, DATE_FORMAT) : "")
                }
                defaultMonth={field.value ? parseStoredDate(field.value) : undefined}
              />
            </PopoverContent>
          </Popover>
          {fieldState.invalid && <FieldError errors={[fieldState.error]} />}
        </Field>
      )}
    />
  );
}
