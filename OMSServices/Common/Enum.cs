namespace OMSServices.Enum
{
    public enum L2OrderInfoLocation
    {
        Top = 0,
        BetweenL1AndL2 = 2,
        BetweenL2AndEntry = 4,
    }

    public enum L2HBAlignment
    {
        Left = 0,
        Right = 1,
        None = 9,
    }

    public enum ActionArg
    {
        Add = 0,
        Modify = 1,
        Remove = 2,
    }

    public enum SelectedSettingTab
    {
        Tickets = 0,
        LevelII = 1,
        MKTWatch = 2,
        Blotter = 3,
        Positions = 4,
        HotKeys = 5,
        Display = 6,
        Locate = 7,
    }

    public enum ENServerReplyOperations
    {
        OPERATION_NONE = 0,
        OPERATION_ETB_VALIDATION,
        OPERATION_RESTRICTED_SYMBOL_VALIDATION,
        OPERATION_LIMIT_VALIDATION,
        OPERATION_LOCATE_QUOTE,
        OPERATION_LOCATE
    }

    public enum ENServerReplyFields
    {
        SERVER_REPLY_TYPE = 1,      // Reply Type : ERROR/WARNING/CONFIRMATION
        SERVER_REPLY_ON_OPERATION,
        SERVER_REPLY_MESSAGE,
        SERVER_REPLY_EXTRA_PARAMS,
        SERVER_REPLY_VALUE_TO_VALIDATE
    }

    public enum ENServerReplyType
    {
        SERVER_REPLY_NONE = 0,
        SERVER_REPLY_WARNING,   // OK|Cancel
        SERVER_REPLY_QUESTION,  // Yes|No
        SERVER_REPLY_ERROR,     // OK
        SERVER_REPLY_SUCCESS,
    }

    public enum ENServerReplyExtraParams
    {
        EXTRA_PARAM_QTY = 1
    }

    public enum ENActionOnServerReply
    {
        ACTION_NONE = 0,
        ACTION_RESEND_TO_SERVER,
        ACTION_ROLLBACK_OPERATION,
        ACTION_FAILED
    }
    public enum QueryType
    {
        Orders,
        OpenOrders,
        Executions,
        Positions,
        BuyingPower,
        Locates,
        LocateSummary,
        Side,
        Destination,
        Account,
        TIF,
        CommType,
        OrdType,
        LocateTIF,
        MktTopPerfCateg,
        MktTopPerfExchange,
        NetLimitSummary,
        CompleteAuditTrail,
        AuditTrail,
        TimeZone,
        OptionOrders,
        LocateSummaryWithSymbol,
        ETBHTB,
        Booths,
    }

    public enum CoveredUnCovered
    {
        COVERED = '0',
        UN_COVERED = '1'
    }
    public enum CustomerFirm
    {
        CUSTOMER = '0',
        FIRM = '1',
        CUST_BD = '2',
        CUST_MM = '3',
        NON_MEMBER_MM = '4',
        FIRM_BD = '5',
        PROPRIETARY_CUSTOMER = '7',
        PROFESSIONAL_CUSTOMER = '8',
    }

    public enum OpenClose
    {
        OPEN = 'O',
        CLOSE = 'C',
    }
    public enum PutCall
    {
        PUT = '0',
        CALL = '1'
    }

    /// <summary>
    /// Request types for QueryServer.
    /// </summary>
    public enum RequestType
    {
        NULL = 0,

        /// <summary>
        /// One time query.
        /// </summary>
        Query = 1,

        /// <summary>
        /// Subscription/Continuous query.
        /// </summary>
        QueryContinuous = 2,

        /// <summary>
        /// Set query.
        /// </summary>
        Set = 3,

        /// <summary>
        /// To unsubscribe a subscribed/continuous query.
        /// </summary>
        Unsubscribe = 4,

        /// <summary>
        /// Remove query.
        /// </summary>
        Remove = 5,
    }
}
