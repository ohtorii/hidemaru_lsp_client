namespace LSP.Model
{
    /**
	 * General parameters to register for a capability.
	 */
    interface IRegistration
    {
        /**
		 * The id used to register the request. The id can be used to deregister
		 * the request again.
		 */
        string id { get; set; }

        /**
		 * The method / capability to register for.
		 */
        string method { get; set; }

        /**
		 * Options necessary for the registration.
		 */
        object registerOptions { get; set; }
    }

    class Registration : IRegistration
    {
        public string id { get; set; }
        public string method { get; set; }
        public object registerOptions { get; set; }
    }
    /*interface IRegistrationParams
	{
		IRegistration[] registrations { get; set; }
	}*/


    class RegistrationParams /*: IRegistrationParams*/
    {
        public Registration[] registrations { get; set; }
    }
}
