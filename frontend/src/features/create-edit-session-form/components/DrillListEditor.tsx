import { Button } from "@/components/ui/button";
import { Field, FieldLabel } from "@/components/ui/field";
import { Input } from "@/components/ui/input";
import type { UseFormRegister, Control, UseFieldArrayReturn } from "react-hook-form";
import type { SessionFormValues } from "../schemas/sessionFormSchema";
import { Trash } from 'lucide-react';

interface DrillListEditorProps {
  register: UseFormRegister<SessionFormValues>;
  control: Control<SessionFormValues>;
  fieldArray: UseFieldArrayReturn<SessionFormValues, "drills">;
}

export function DrillListEditor({ register, fieldArray }: DrillListEditorProps) {
  const { fields, append, remove } = fieldArray;

  return (
    <div className="space-y-3">
      {fields.map((field, index) => (
        <div key={field.id} className="flex gap-2 items-end">
          <Field className="flex-1">
            <FieldLabel htmlFor={`drills.${index}.name`}>Drill Name</FieldLabel>
            <Input id={`drills.${index}.name`} {...register(`drills.${index}.name`)} />
          </Field>
          <Field className="w-32">
            <FieldLabel htmlFor={`drills.${index}.completionTimeInSeconds`}>Seconds</FieldLabel>
            <Input
              id={`drills.${index}.completionTimeInSeconds`}
              type="number"
              {...register(`drills.${index}.completionTimeInSeconds`, { valueAsNumber: true })}
            />
          </Field>
          <Button type="button" variant="destructive" onClick={() => remove(index)}>
            <Trash data-icon="inline-start" />
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