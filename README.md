# ASP.NET Core / WebAssembly Blazor Example Project

This example project is a simplified management of assets and parts in a system and it would be part of a CMMS software (computerized maintenance management system). The example pregenerates a simple sortation conveyor system for a BHS (baggage handling system) and some common parts for conveyor. The example has two pages, assets and parts.

## Assets Page
The assets page allows the user to add/edit/delete assets. Assets can be arrange in a hierarchy manner and an asset either represents important equipment in the system, an area for inventory storage or a logical grouping for the children underneath it. Basic information can be entered about the asset and for equipment assets, the asset can taken offline or 
put back online. Also, the user can add/edit/delete storage locations for an area asset.

<img width="1919" alt="image" src="https://github.com/user-attachments/assets/8b9a612c-ec2a-4f99-a123-8564d89e7ed4" />

### Add Asset

On the assets page, the user can create a new asset.

* Name - A friendly name for the asset; required and must be unique.
* Description - A description about the asset; optional.
* Type of Asset - The type the asset will represent; required. Area is a location for part storage. Group is an abstract grouping of assets for the system. Equipment is a physical equipment in the system.
* Parent Asset - The parent for this asset; optional.
* Category - The category for the asset; optional.

<img width="469" alt="image" src="https://github.com/user-attachments/assets/71273346-4629-4ce9-be85-43b49fd91ea6" />

### Edit Asset

On the assets page, the user can edit an existing asset; .

<img width="1919" alt="image" src="https://github.com/user-attachments/assets/7fe824d0-eb34-4793-9ac6-4c4b0e84b17f" />

<img width="1919" alt="image" src="https://github.com/user-attachments/assets/00172540-fb4d-4089-a95f-94a3635c8f2e" />

<img width="467" alt="image" src="https://github.com/user-attachments/assets/0a65b87f-ce7c-45b8-96e7-74d88caf865b" />

<img width="466" alt="image" src="https://github.com/user-attachments/assets/ecd2a494-9d4f-4f58-8592-a10b9f864615" />

## Parts Page
The parts page allows the user to add/edit/delete parts used for maintenance purposes on the equipment assets the CMMS keeps track of. Basic information can be entered about the part. Also, the user can add/edit/delete part inventory associated with a storage location of an area asset.

<img width="1919" height="977" alt="image" src="https://github.com/user-attachments/assets/c573af18-9612-44cf-a697-fd405244ce91" />

### Add Part

On the parts page, the user can create a new part used for maintenance.

* Name - A friendly name for the part; required and must be unique.
* Description - A description about the part; optional.
* Category - The category for the part; optional.

<img width="467" alt="image" src="https://github.com/user-attachments/assets/73d78d96-6eab-4622-bdc0-a9b9b6f990bd" />

### Delete Part

On the parts page, the user can delete a part. The user will be required to confirm the deletion and once confirmed, the part and its stock will be deleted.

<img width="479" height="140" alt="image" src="https://github.com/user-attachments/assets/06e3c7ed-2f16-4ab6-8fe2-11fddd7511a9" />

If another user already deteled the part, this message will be displayed.

<img width="474" height="153" alt="image" src="https://github.com/user-attachments/assets/b22fd644-89b3-4d5d-b14f-5e17038d0e40" />

### Part Detail Page

On the parts page, the user can navigate to the part detail page where the user can view/edit the part and its inventory.

<img width="1919" alt="image" src="https://github.com/user-attachments/assets/c974362b-0560-469b-8a22-5de68241f545" />

#### Edit Part

On the part detail page, the user can edit the top panel which contains general information about the part.

* Name - A friendly name for the part; required and must be unique.
* Category - The category for the part; optional.
* Manufacturer - Who creates the part; optional.
* Manufacturer Number - The part identifier assigned by the manufacturer; optional.
* Model - The model of the part; optional.
* Make - The make of the part; optional.
* Description - A description about the part; optional.
* Obsolete - Marks if the part not be used anymore.

If another user updated the part while you're editing it, this message will be displayed.

<img width="467" height="146" alt="image" src="https://github.com/user-attachments/assets/47698be3-0356-4aeb-9c7f-82a61bdaead7" />

If another user already deleted the part, this message will be displayed.

<img width="474" height="153" alt="image" src="https://github.com/user-attachments/assets/b22fd644-89b3-4d5d-b14f-5e17038d0e40" />

#### Add/Edit Part Stock

On the part detail page, the user can create new inventory at a storage location or edit the amount at an existing location from the bottom panel.

* Storage Location - The location the part will be stored; only required and editable when adding new inventory.
* Amount - The number of parts stored at the location; zero or more than 0.

<img width="468" alt="image" src="https://github.com/user-attachments/assets/40dd4aa8-4f9a-4661-9f6e-937dd308b66c" />

<img width="465" alt="image" src="https://github.com/user-attachments/assets/46ecaf2c-972b-4656-ab11-ce579bc5ae66" />

If another user updated the stock while you're editing it, this message will be displayed.

<img width="471" height="209" alt="image" src="https://github.com/user-attachments/assets/80cae001-c765-468e-ae01-dc42d7a15133" />

If another user already deteled the stock, this message will be displayed.

<img width="463" height="212" alt="image" src="https://github.com/user-attachments/assets/646328b5-4960-42e0-942b-5d66c2592e91" />

#### Delete Part Stock

On the part detail page, the user can delete stock at a location from the bottom panel. The user will be required to confirm the deletion and once confirmed, the stock will be deleted at the location.

<img width="479" height="140" alt="image" src="https://github.com/user-attachments/assets/06e3c7ed-2f16-4ab6-8fe2-11fddd7511a9" />

If another user already deteled the stock, a message will be displayed.

<img width="469" height="148" alt="image" src="https://github.com/user-attachments/assets/32826bed-ad58-422e-afb5-af069f2fcefb" />

#### Part Not Found

This message is displayed when navigating to the part detail page and the part is not found.

<img width="1919" height="979" alt="image" src="https://github.com/user-attachments/assets/0040ba51-d579-4644-9df1-edcb34824302" />


