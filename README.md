# README #

The users are the same; vendor:vendor and admin:admin

Added localization in English and Spanish, with English as default

I created a "List View" User Control and an "Edit" User Control for Shows, Clients and Reservations. The PriceLists seemed to me intrinsical to the shows so I left the option to edit them in the EditShow User Control.

I respected the use of messages as the way of communication between the tabs but as I saw with you in class, whenever I embedded a User Control into another (Like the ReservationsView inside the ClientEdit UserControl) I allowed the container user control to access directly the methods and properties of the embedded User Control.

Also with respect to inputs, I tried to follow a philosophy of not letting the user input anything incorrect, like trying to type letters in a price or postal code.