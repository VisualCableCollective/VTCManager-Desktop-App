import React from "react";
import Link from "next/link";
import { UpperInfoWidget } from "../components/widgets/UpperInfoWidget";
import { VehicleWidget } from "../components/widgets/VehicleWidget";
import { CenterWidgetContainer } from "../components/widgets/CenterWidgetContainer";

function Dashboard() {
  return (
    <div className="w-full h-full bg-dark-1 flex p-6">
      <div className="w-2/3">
        <CenterWidgetContainer />
      </div>
      <VehicleWidget />
    </div>
  );
}

export default Dashboard;
