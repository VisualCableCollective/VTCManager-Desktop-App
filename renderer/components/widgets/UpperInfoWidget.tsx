import { WidgetBase } from "../shared/WidgetBase";

// icons
import { FaGasPump } from "react-icons/fa";
import { WiDayRain } from "react-icons/wi";
import {useTelemetry} from "../../contexts/TelemetryContext";

export function UpperInfoWidget() {
  const telemetry = useTelemetry();
  const currentTime = new Date();
  return (
    <WidgetBase className="p-5">
      <div className="grid grid-cols-3 gap-2 lg:gap-28">
        <div className="flex items-center">
          <div className="text-xl mr-2">
            <FaGasPump />
          </div>
          <WidgetDefaultText>{telemetry.data ? Math.floor((telemetry.data.truck.fuel.value / telemetry.data.truck.fuel.capacity) * 100)+ "%" : "n/a"}</WidgetDefaultText>
        </div>
        <div className="flex justify-between">
          <div className="flex items-center">
            <div className="text-2xl mr-2">
              {
                // <WiDayRain />
              }
            </div>
            {
              //<WidgetDefaultText>Raining</WidgetDefaultText>
            }
          </div>
          {//<WidgetDefaultText>25 C</WidgetDefaultText>
          }
        </div>
        <div className="flex justify-between">
          <WidgetDefaultText>{currentTime.toDateString()}</WidgetDefaultText>
          <WidgetDefaultText>{currentTime.getHours() + ":" + currentTime.getMinutes()}</WidgetDefaultText>
        </div>
      </div>
    </WidgetBase>
  );
}

// ToDo: move this
interface WDTProps {
  children: React.ReactNode;
}
function WidgetDefaultText(props: WDTProps) {
  return <h1 className="text-opacity-80 text-white">{props.children}</h1>;
}
