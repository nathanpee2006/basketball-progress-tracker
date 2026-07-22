import { Field, FieldLabel } from "@/components/ui/field";
import { Input } from "@/components/ui/input";
import type { UseFormRegister, FieldErrors } from "react-hook-form";
import type { SessionFormValues } from "../schemas/sessionFormSchema";

interface FreeThrowInputsProps {
  register: UseFormRegister<SessionFormValues>;
  errors: FieldErrors<SessionFormValues>;
}

export function FreeThrowInputs({ register, errors }: FreeThrowInputsProps) {
  return (
    <div className="space-y-4">
      <Field>
        <FieldLabel htmlFor="freeThrowMakes">Free Throw Makes</FieldLabel>
        <Input
          id="freeThrowMakes"
          type="number"
          {...register("freeThrowMakes", { valueAsNumber: true })}
        />
        {errors.freeThrowMakes && (
          <span className="text-sm text-destructive">{errors.freeThrowMakes.message}</span>
        )}
      </Field>

      <Field>
        <FieldLabel htmlFor="freeThrowAttempts">Free Throw Attempts</FieldLabel>
        <Input
          id="freeThrowAttempts"
          type="number"
          {...register("freeThrowAttempts", { valueAsNumber: true })}
        />
        {errors.freeThrowAttempts && (
          <span className="text-sm text-destructive">{errors.freeThrowAttempts.message}</span>
        )}
      </Field>
    </div>
  );
}