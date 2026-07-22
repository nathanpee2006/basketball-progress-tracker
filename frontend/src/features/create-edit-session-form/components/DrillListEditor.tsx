import { Button } from "@/components/ui/button";
import { Field, FieldLabel } from "@/components/ui/field";
import { Input } from "@/components/ui/input";
import type {
  UseFormRegister,
  UseFieldArrayReturn,
  FieldErrors,
} from "react-hook-form";
import type { SessionFormValues } from "../schemas/sessionFormSchema";
import { Trash } from "lucide-react";

interface DrillListEditorProps {
  register: UseFormRegister<SessionFormValues>;
  fieldArray: UseFieldArrayReturn<SessionFormValues, "drills">;
  errors: FieldErrors<SessionFormValues>;
}

export function DrillListEditor({
  register,
  fieldArray,
  errors,
}: DrillListEditorProps) {
  const { fields, append, remove } = fieldArray;

  return (
    <div className="space-y-3">
      {fields.map((field, index) => (
        <div key={field.id} className="flex gap-2 items-end">
          <Field className="flex-1">
            <FieldLabel htmlFor={`drills.${index}.name`}>Drill Name</FieldLabel>
            <Input
              id={`drills.${index}.name`}
              {...register(`drills.${index}.name`)}
            />
            <span className="text-sm text-destructive min-h-5 block">
              {errors.drills?.[index]?.name?.message}
            </span>
          </Field>

          <Field className="w-32">
            <FieldLabel htmlFor={`drills.${index}.completionTimeInSeconds`}>
              Seconds
            </FieldLabel>
            <Input
              id={`drills.${index}.completionTimeInSeconds`}
              type="number"
              {...register(`drills.${index}.completionTimeInSeconds`, {
                valueAsNumber: true,
              })}
            />
            <span className="text-sm text-destructive min-h-5 block">
              {errors.drills?.[index]?.completionTimeInSeconds?.message}
            </span>
          </Field>

          <Button
            type="button"
            variant="destructive"
            aria-label={`Remove drill ${index + 1}`}
            onClick={() => remove(index)}
          >
            <Trash
              data-icon="inline-start"
              aria-hidden="true"
              focusable="false"
            />
            X
          </Button>
        </div>
      ))}
      <Button
        type="button"
        variant="outline"
        onClick={() => append({ name: "", completionTimeInSeconds: 0 })}
      >
        Add Drill
      </Button>
    </div>
  );
}
