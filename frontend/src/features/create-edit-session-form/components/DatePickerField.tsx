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
import { format } from "date-fns/format";

interface DatePickerFieldProps {
  control: Control<SessionFormValues>;
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
                    format(new Date(field.value), "PPP")
                  ) : (
                    <span>Pick a date</span>
                  )}
                </Button>
              }
            />
            <PopoverContent className="w-auto p-0" align="start">
              <Calendar
                mode="single"
                selected={field.value ? new Date(field.value) : undefined}
                onSelect={(date) =>
                  field.onChange(date ? date.toISOString().slice(0, 10) : "")
                }
                defaultMonth={field.value ? new Date(field.value) : undefined}
              />
            </PopoverContent>
          </Popover>
          {fieldState.invalid && <FieldError errors={[fieldState.error]} />}
        </Field>
      )}
    />
  );
}
