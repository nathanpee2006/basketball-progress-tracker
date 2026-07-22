import { useForm, useFieldArray } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useParams, useNavigate } from "react-router";
import {
  sessionFormSchema,
  type SessionFormValues,
} from "./schemas/sessionFormSchema";
import { ZoneInputs } from "./components/ZoneInputs";
import { FreeThrowInputs } from "./components/FreeThrowInputs";
import { Button } from "@/components/ui/button";
import { DrillListEditor } from "./components/DrillListEditor";
import { DatePickerField } from "./components/DatePickerField";
import { CourtVisualization } from "@/components/session/CourtVisualization";
import { zoneFieldsToZoneStats, type ZoneId } from "@/types/court";
import { useCreateSession } from "./createSession";
import { toast } from "sonner";
import { Spinner } from "@/components/ui/spinner";

type SessionFormPageProps = {
  mode: "create" | "edit";
};

const defaultValues: SessionFormValues = {
  date: "",
  paintMakes: 0,
  paintAttempts: 0,
  midrangeMakes: 0,
  midrangeAttempts: 0,
  threePointMakes: 0,
  threePointAttempts: 0,
  freeThrowMakes: 0,
  freeThrowAttempts: 0,
  drills: [],
};

export function SessionFormPage({ mode }: SessionFormPageProps) {
  const { id } = useParams();
  const navigate = useNavigate();

  const form = useForm<SessionFormValues>({
    resolver: zodResolver(sessionFormSchema),
    defaultValues,
    // TODO if mode === "edit": fetch existing session and pass via `values` prop instead of defaultValues, so the form updates once data loads
  });

  const {
    register,
    control,
    handleSubmit,
    watch,
    setValue,
    formState: { errors },
  } = form;
  const watchedZoneFields = watch([
    "paintMakes",
    "paintAttempts",
    "midrangeMakes",
    "midrangeAttempts",
    "threePointMakes",
    "threePointAttempts",
    "freeThrowMakes",
    "freeThrowAttempts",
  ]);
  const zones = zoneFieldsToZoneStats({
    paintMakes: watchedZoneFields[0],
    paintAttempts: watchedZoneFields[1],
    midrangeMakes: watchedZoneFields[2],
    midrangeAttempts: watchedZoneFields[3],
    threePointMakes: watchedZoneFields[4],
    threePointAttempts: watchedZoneFields[5],
    freeThrowMakes: watchedZoneFields[6],
    freeThrowAttempts: watchedZoneFields[7],
  });

  const handleZoneChange = (
    zoneId: ZoneId,
    makes: number,
    attempts: number,
  ) => {
    const fieldMap: Record<
      ZoneId,
      [keyof SessionFormValues, keyof SessionFormValues]
    > = {
      paint: ["paintMakes", "paintAttempts"],
      midrange: ["midrangeMakes", "midrangeAttempts"],
      threePoint: ["threePointMakes", "threePointAttempts"],
      freeThrow: ["freeThrowMakes", "freeThrowAttempts"],
    };
    const [makesField, attemptsField] = fieldMap[zoneId];
    setValue(makesField, makes, { shouldValidate: true, shouldDirty: true });
    setValue(attemptsField, attempts, {
      shouldValidate: true,
      shouldDirty: true,
    });
  };

  const drillFieldArray = useFieldArray({ control, name: "drills" });

  const { createSession, isLoading: isCreating, error: createError } = useCreateSession();

  const onSubmit = async (data: SessionFormValues) => {
    if (mode === "create") {
      try {
        await createSession(data);
        navigate("/sessions");
        toast.success("Session created successfully!");
      } catch (error) {
        console.error("Error creating session:", createError?.message);
        toast.error("Failed to create session.");
      }
      
    } else {
      // PUT request to /sessions/:id
    }
  };

  return (
    <section className="space-y-2">
      <h2 className="text-xl font-semibold">
        {mode === "create" ? "New Session" : "Edit Session"}
      </h2>

      <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
        <CourtVisualization
          mode="edit"
          zones={zones}
          onZoneChange={handleZoneChange}
        />
        <DatePickerField control={control} />
        <ZoneInputs register={register} errors={errors} />
        <FreeThrowInputs register={register} errors={errors} />
        <DrillListEditor
          control={control}
          register={register}
          fieldArray={drillFieldArray}
        />

        <div className="flex gap-3">
          <Button type="submit" disabled={isCreating}>
            <Spinner className={`mr-2 h-4 w-4 animate-spin ${isCreating ? "inline-block" : "hidden"}`} data-icon="inline-start" />
            {isCreating ? "Saving..." : "Save"}
          </Button>
          <Button type="button" variant="destructive" onClick={() => navigate("/sessions")}>
            Cancel
          </Button>
        </div>
      </form>
    </section>
  );
}
