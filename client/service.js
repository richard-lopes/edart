$(function () {



    $.getOffers = function() {
        var offers = [];

        $.ajax({
            url: "http://10.72.20.14/Edart/api/Listings/GetCurrentListings?token=sa",
            type: "GET",
            dataType: "json",
            success: function(data) {
				
				for(var i=0; i<data.length; i++)
				{
					var offer = data[i];
					var newoffer = {};
					newoffer.id = offer.Id;
					newoffer.title = offer.Title;
					newoffer.header = offer.Title;
					newoffer.location = offer.Location;
					newoffer.description = offer.Description;
					newoffer.owner = offer.UserName;
					newoffer.photo = offer.Photo;
					offers.push(newoffer);
				}    
            },
            error: function(xhr, textStatus, errorThrown) {
                alert('error');
            }
        });
		return offers;
    };
	
    

    


});
