using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xavor.SD.Model;

namespace Xavor.SD.BusinessLayer
{
    public interface IRuleEngineBL
    {
        Ruleengine InsertRuleEngine(Ruleengine ruleEngine);
        Ruleengine UpdateRuleEngine(Ruleengine ruleEngine);
        IEnumerable<Ruleengine> GetRuleEngine();
        bool DeleteRuleEngine(int ruleEngineId);
        Ruleengine GetRuleEngine(int Id);
        IQueryable<Ruleengine> QueryRuleEngine();
        Ruleengine GetRuleEngineByCustomerId(int customerId);
    }
}
